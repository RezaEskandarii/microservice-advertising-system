package repositories

import (
	"database/sql"
	"errors"
	"sync"
	"time"
	"wallet-api/internal/application_errors"
	"wallet-api/internal/enums/transaction_types"
	"wallet-api/internal/models"

	_ "github.com/lib/pq"
)

type WalletRepository interface {
	Deposit(userID string, amount float64, idempotencyKey string) error
	Withdraw(userID string, amount float64, idempotencyKey string) error
	GetTransactions(userID string) ([]models.Transaction, error)
	GetAmount(userID string) (float64, error)
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
	if err == application_errors.DuplicatedRequestError {
		return nil
	}

	res, err := r.db.Exec("INSERT INTO wallets (user_id,balance,created_at) VALUES ($1,$2,$3) ON CONFLICT (user_id) DO UPDATE SET balance = (select balance from wallets where user_id = $1) + $2", userID, amount, time.Now())
	if err != nil {
		return err
	}

	n, err := res.RowsAffected()
	if err != nil {
		return err
	}
	if n == 0 {
		return application_errors.InsufficientWalletBalance
	}

	_, err = r.db.Exec("INSERT INTO transactions (user_id, amount, type) VALUES ($1, $2, $3)", userID, amount, transaction_types.Deposit)
	if err != nil {
		return err
	}

	return nil
}

// Withdraw  subtracts the specified amount from the user's wallet.
func (r *PostgreSQLRepository) Withdraw(userID string, amount float64, idempotencyKey string) error {
	err := r.checkIdempotency(userID, idempotencyKey)
	if err == nil {
		res, err := r.db.Exec("UPDATE wallets SET balance = balance - $2 WHERE user_id = $1 AND balance >= $2", userID, amount)
		if err != nil {
			return err
		}

		n, err := res.RowsAffected()
		if err != nil {
			return err
		}
		if n == 0 {
			return errors.New("insufficient funds")
		}

		_, err = r.db.Exec("INSERT INTO transactions (id, user_id, amount, type) VALUES (NULL,$1, $2, $3)", userID, amount, transaction_types.Withdraw)
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

// checkIdempotency checks if the idempotency key has been used before.
func (r *PostgreSQLRepository) checkIdempotency(userID string, idempotencyKey string) error {
	var exists bool
	err := r.db.QueryRow("SELECT EXISTS(SELECT 1 FROM idempotent_history WHERE user_id = $1 AND idempotent_key=$2)", userID, idempotencyKey).Scan(&exists)
	if err != nil {
		return err
	}
	if exists {
		return application_errors.DuplicatedRequestError
	}

	_, err = r.db.Exec("INSERT INTO  idempotent_history(user_id,idempotent_key) VALUES ($1,$2)", userID, idempotencyKey)
	if err != nil {
		return err
	}

	return nil
}
