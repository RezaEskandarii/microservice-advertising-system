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
	Withdraw(userID string, amount float64, idempotencyKey string) error
	GetTransactions(userID string) ([]models.Transaction, error)
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
	err := r.checkIdempotency(userID, idempotencyKey)
	if err == app_errors.DuplicatedRequestError {
		return nil
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

	res, err := tx.Exec(`  INSERT INTO wallets (user_id, balance, created_at)  
  		VALUES ($1, $2, $3)  ON CONFLICT (user_id) 
        DO UPDATE SET balance = wallets.balance + EXCLUDED.balance`,
		userID, amount, time.Now())
	if err != nil {
		return err
	}

	n, err := res.RowsAffected()
	if err != nil {
		return err
	}
	if n == 0 {
		return app_errors.InsufficientWalletBalanceError
	}

	_, err = tx.Exec(`
        INSERT INTO transactions (user_id, amount, type) 
        VALUES ($1, $2, $3)`, userID, amount, transaction_types.Deposit)
	if err != nil {
		return err
	}

	err = tx.Commit()
	if err != nil {
		return err
	}

	return nil
}

// Withdraw  subtracts the specified amount from the user's wallet.
func (r *PostgreSQLRepository) Withdraw(userID string, amount float64, idempotencyKey string) error {
	// Check idempotency before proceeding
	err := r.checkIdempotency(userID, idempotencyKey)
	if err == nil {
		// Start a new transaction
		tx, err := r.db.Begin()
		if err != nil {
			return err
		}

		// Ensure rollback in case of any error
		defer func() {
			if err != nil {
				tx.Rollback()
			}
		}()

		// Update wallet balance
		res, err := tx.Exec("UPDATE wallets SET balance = balance - $2 WHERE user_id = $1 AND balance >= $2", userID, amount)
		if err != nil {
			return err
		}

		// Check if the update was successful
		n, err := res.RowsAffected()
		if err != nil {
			return err
		}
		if n == 0 {
			return errors.New("insufficient funds")
		}

		// Insert the transaction record
		_, err = tx.Exec("INSERT INTO transactions (user_id, amount, type) VALUES ($1, $2, $3)", userID, amount, transaction_types.Withdraw)
		if err != nil {
			return err
		}

		// Commit the transaction
		err = tx.Commit()
		if err != nil {
			return err
		}
	}

	return err
}

// GetTransactions retrieves the transaction history for a user.
func (r *PostgreSQLRepository) GetTransactions(userID string) ([]models.Transaction, error) {
	rows, err := r.db.Query("SELECT id, user_id, amount, type FROM transactions WHERE user_id = $1 ORDER BY created_at DESC", userID)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var transactions []models.Transaction
	for rows.Next() {
		var txn models.Transaction
		err := rows.Scan(&txn.ID, &txn.UserID, &txn.Amount, &txn.Type)
		if err != nil {
			return nil, err
		}
		transactions = append(transactions, txn)
	}

	return transactions, nil
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

	_, err = r.db.Exec("INSERT INTO  idempotent_history(user_id,idempotent_key) VALUES ($1,$2)", userID, idempotencyKey)
	if err != nil {
		return err
	}

	return nil
}
