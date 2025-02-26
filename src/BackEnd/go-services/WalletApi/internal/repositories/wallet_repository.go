package repositories

import (
	"database/sql"
	"errors"
	"google.golang.org/genproto/googleapis/type/decimal"
	"sync"
	"time"
	"wallet-api/internal/app_errors"
	"wallet-api/internal/constants/transaction_types"
	"wallet-api/internal/models"

	_ "github.com/lib/pq"
)

type WalletRepository interface {
	Deposit(userID string, amount float64, idempotencyKey string) error
	Withdrawal(userID string, amount float64, idempotencyKey string) error
	GetTransactions(userID string, page, perPage int) (models.PaginatedData[models.Transaction], error)
	GetAmount(userID string) (float64, error)
	RemoveExpire(createdAt time.Time) error
	GetTransactionsReport(yearNumber int) (map[int]decimal.Decimal, error)
}

// PostgreSQLRepository is an implementation of the WalletRepository interface using PostgreSQL.
type PostgreSQLRepository struct {
	db   *sql.DB
	lock *sync.Mutex
}

// NewPostgreSQLRepository creates a new instance of PostgreSQLRepository.
func NewPostgreSQLRepository(db *sql.DB) *PostgreSQLRepository {
	return &PostgreSQLRepository{
		db:   db,
		lock: &sync.Mutex{},
	}
}

// Deposit adds the specified amount to the user's wallet.
func (r *PostgreSQLRepository) Deposit(userID string, amount float64, idempotencyKey string) error {
	// Check idempotency before proceeding
	if err := r.checkIdempotency(userID, idempotencyKey); err != nil {
		if errors.Is(err, app_errors.DuplicatedRequestError) {
			return nil
		}
		return err
	}

	tx, err := r.db.Begin()
	if err != nil {
		return err
	}

	defer func() {
		if err != nil {
			tx.Rollback()
		}
	}()

	// Insert idempotency record to prevent duplicate transactions
	if err = insertIdempotency(userID, idempotencyKey, tx); err != nil {
		return err
	}

	// Lock the row for update to prevent race conditions
	var balance float64
	err = tx.QueryRow(`
		SELECT balance FROM wallets 
		WHERE user_id = $1 FOR UPDATE`, userID).Scan(&balance)
	if err != nil {
		// If no wallet exists, insert a new one
		if err == sql.ErrNoRows {
			_, err = tx.Exec(`
				INSERT INTO wallets (user_id, balance, created_at)  
				VALUES ($1, $2, $3)`, userID, amount, time.Now())
			if err != nil {
				return err
			}
		} else {
			return err
		}
	} else {
		// If wallet exists, update balance safely
		_, err = tx.Exec(`
			UPDATE wallets SET balance = balance + $1 WHERE user_id = $2`, amount, userID)
		if err != nil {
			return err
		}
	}

	// Insert transaction record
	_, err = tx.Exec(`
		INSERT INTO transactions (user_id, amount, type) 
		VALUES ($1, $2, $3)`, userID, amount, transaction_types.Deposit)
	if err != nil {
		return err
	}

	// Commit the transaction
	if err = tx.Commit(); err != nil {
		return err
	}

	return nil
}

// Withdrawal  subtracts the specified amount from the user's wallet.
func (r *PostgreSQLRepository) Withdrawal(userID string, amount float64, idempotencyKey string) error {
	// Check idempotency before proceeding
	if err := r.checkIdempotency(userID, idempotencyKey); err != nil {
		if err == app_errors.DuplicatedRequestError {
			return nil
		} else {
			return err
		}
	}

	tx, err := r.db.Begin()
	if err != nil {
		return err
	}

	defer func() {
		if err != nil {
			tx.Rollback()
		}
	}()

	if err = insertIdempotency(userID, idempotencyKey, tx); err != nil {
		return err
	}

	// Lock the wallet row
	var currentBalance float64
	err = tx.QueryRow("SELECT balance FROM wallets WHERE user_id = $1 FOR UPDATE", userID).Scan(&currentBalance)
	if err != nil {
		return err
	}

	if currentBalance < amount {
		return errors.New("insufficient funds")
	}

	// Update wallet balance
	_, err = tx.Exec("UPDATE wallets SET balance = balance - $2 WHERE user_id = $1", userID, amount)
	if err != nil {
		return err
	}

	_, err = tx.Exec("INSERT INTO transactions (user_id, amount, type) VALUES ($1, $2, $3)", userID, amount, transaction_types.Withdrawal)
	if err != nil {
		return err
	}

	// Commit the transaction
	err = tx.Commit()
	if err != nil {
		return err
	}

	return err
}

// GetTransactions retrieves the transaction history for a user.
func (r *PostgreSQLRepository) GetTransactions(userID string, page, perPage int) (models.PaginatedData[models.Transaction], error) {

	offset := (page - 1) * perPage

	var totalItems int
	err := r.db.QueryRow("SELECT COUNT(*) FROM transactions WHERE user_id = $1", userID).Scan(&totalItems)
	if err != nil {
		return models.PaginatedData[models.Transaction]{}, err
	}

	rows, err := r.db.Query(`
		SELECT id, user_id, amount, type 
		FROM transactions 
		WHERE user_id = $1 
		ORDER BY created_at DESC 
		LIMIT $2 OFFSET $3`, userID, perPage, offset)
	if err != nil {
		return models.PaginatedData[models.Transaction]{}, err
	}
	defer rows.Close()

	var transactions []models.Transaction
	for rows.Next() {
		var txn models.Transaction
		err := rows.Scan(&txn.ID, &txn.UserID, &txn.Amount, &txn.Type)
		if err != nil {
			return models.PaginatedData[models.Transaction]{}, err
		}
		transactions = append(transactions, txn)
	}

	return models.PaginateData(transactions, totalItems, page, perPage), nil
}

// GetAmount retrieves the current amount in a user's wallet.
func (r *PostgreSQLRepository) GetAmount(userID string) (float64, error) {
	var amount float64
	err := r.db.QueryRow("SELECT balance FROM wallets WHERE user_id = $1", userID).Scan(&amount)
	if err != nil {
		return 0, err
	}
	return amount, nil
}

// RemoveExpire remove expired idempotent_history
func (r *PostgreSQLRepository) RemoveExpire(createdAt time.Time) error {
	// Extract the date part from the given date
	formattedDate := createdAt.Format("2006-01-02")

	// check if has any row
	var hasRow bool
	db := r.db
	err := db.QueryRow("SELECT EXISTS(SELECT 1 FROM idempotent_history WHERE DATE(created_at) <= $1)", formattedDate).Scan(&hasRow)
	if err != nil {
		return err
	}

	// If count is greater than 0, execute the delete query
	if hasRow {
		query := "DELETE FROM idempotent_history WHERE DATE(created_at) <= $1"
		err = db.QueryRow(query, formattedDate).Err()
		if err != nil {
			return err
		}
	}

	return nil
}

func (r *PostgreSQLRepository) GetTransactionsReport(yearNumber int) (map[int]decimal.Decimal, error) {
	var firstDayOfYar = time.Date(yearNumber, time.January, 1, 0, 0, 0, 0, time.Local)
	var lastDayOfYar = time.Date(yearNumber+1, time.January, 1, 0, 0, 0, -1, time.Local)

	var query = `
		SELECT EXTRACT(YEAR FROM "created_at") AS created_date,
			SUM("amount") FROM transactions
		WHERE "created_at" >= $1 AND "created_at" <= $2
			 GROUP BY  EXTRACT(YEAR FROM "created_at")
`
	rows, err := r.db.Query(query, firstDayOfYar, lastDayOfYar)
	if err != nil && err != sql.ErrNoRows {
		return nil, err
	}
	defer rows.Close()

	var result = make(map[int]decimal.Decimal)
	for rows.Next() {
		var year int
		var sumAmount decimal.Decimal

		err := rows.Scan(&year, &sumAmount)
		if err != nil {
			return nil, err
		}
		result[year] = sumAmount
	}
	return result, err
}

// checkIdempotency checks if the idempotency key has been used before.
func (r *PostgreSQLRepository) checkIdempotency(userID string, idempotencyKey string) error {
	var exists bool
	err := r.db.QueryRow("SELECT EXISTS(SELECT 1 FROM idempotent_history WHERE user_id = $1 AND idempotent_key=$2)", userID, idempotencyKey).Scan(&exists)
	if err != nil {
		return err
	}
	if exists {
		return app_errors.DuplicatedRequestError
	}
	return nil
}

func insertIdempotency(userID string, idempotencyKey string, db *sql.Tx) error {
	_, err := db.Exec("INSERT INTO  idempotent_history(user_id,idempotent_key) VALUES ($1,$2)", userID, idempotencyKey)
	if err != nil {
		db.Rollback()
		return err
	}
	return nil
}
