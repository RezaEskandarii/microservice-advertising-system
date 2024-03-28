package repositories

import (
	"database/sql"
	"errors"
	"wallet-api/internal/models"
)
import (
	_ "github.com/lib/pq"
)

var (
	DuplicatedRequestError = errors.New("duplicate request")
)

type WalletRepository interface {
	Deposit(userID string, amount float64, idempotencyKey string) error
	Withdraw(userID string, amount float64, idempotencyKey string) error
	GetTransactions(userID string) ([]models.Transaction, error)
	GetAmount(userID string) (float64, error)
}

// PostgreSQLRepository is an implementation of the WalletRepository interface using PostgreSQL.
type PostgreSQLRepository struct {
	db *sql.DB
}

// NewPostgreSQLRepository creates a new instance of PostgreSQLRepository.
func NewPostgreSQLRepository(db *sql.DB) *PostgreSQLRepository {
	return &PostgreSQLRepository{
		db: db,
	}
}

// Deposit adds the specified amount to the user's wallet.
func (r *PostgreSQLRepository) Deposit(userID string, amount float64, idempotencyKey uuid.UUID) error {
	err := r.checkIdempotency(userID, idempotencyKey)
	if err == DuplicatedRequestError {
		return nil
	}

	_, err = r.db.Exec("INSERT INTO wallets (user_id,balance) VALUES ($1,$2) ON CONFLICT (user_id) DO UPDATE SET balance = (select balance from wallets where user_id = $1) + $2", userID, amount)
	if err != nil {
		return err
	}

	_, err = r.db.Exec("INSERT INTO transactions (id, user_id, amount, type) VALUES (NULL, $1, $2, 'deposit')", userID, amount)
	if err != nil {
		return err
	}

	return nil
}

// Withdraw subtracts the specified amount from the user's wallet.
func (r *PostgreSQLRepository) Withdraw(userID string, amount float64, idempotencyKey string) error {
	err := r.checkIdempotency(userID, idempotencyKey)
	if err == DuplicatedRequestError {
		return nil
	}

	_, err = r.db.Exec("UPDATE wallets SET amount = amount - $2 WHERE user_id = $1 AND amount >= $2", userID, amount)
	if err != nil {
		return err
	}

	if r.dbChanges() == 0 {
		return errors.New("insufficient funds")
	}

	_, err = r.db.Exec("INSERT INTO transactions (id, user_id, amount, type) VALUES (NULL,$1, $2, 'withdraw')", userID, amount)
	if err != nil {
		return err
	}

	return nil
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
	err := r.db.QueryRow("SELECT amount FROM wallets WHERE user_id = $1", userID).Scan(&amount)
	if err != nil {
		return 0, err
	}
	return amount, nil
}

// checkIdempotency checks if the idempotency key has been used before.
func (r *PostgreSQLRepository) checkIdempotency(userID string, idempotencyKey string) error {
	var exists bool
	err := r.db.QueryRow("SELECT EXISTS(SELECT 1 FROM idempotency WHERE user_id = $1 AND idemotent_key=$2)", userID, idempotencyKey).Scan(&exists)
	if err != nil {
		return err
	}
	if exists {
		return DuplicatedRequestError
	}

	_, err = r.db.Exec("INSERT INTO idempotencies (user_id,idempotent_history) VALUES ($1,$2)", userID, idempotencyKey)
	if err != nil {
		return err
	}

	return nil
}

// dbChanges returns the number of rows affected by the last database operation.
func (r *PostgreSQLRepository) dbChanges() int64 {
	var changes int64
	r.db.QueryRow("SELECT SUM(changed_rows) FROM pg_stat_database WHERE datname = current_database()").Scan(&changes)
	return changes
}
