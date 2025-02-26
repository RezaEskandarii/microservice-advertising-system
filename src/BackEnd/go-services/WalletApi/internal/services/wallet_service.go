package services

import (
	"fmt"
	"github.com/robfig/cron"
	"google.golang.org/genproto/googleapis/type/decimal"
	"log"
	"time"
	"wallet-api/internal/models"
	"wallet-api/internal/repositories"
)

type WalletService interface {
	// Deposit adds the specified amount to the user's wallet balance.
	// The idempotencyKey parameter is used to ensure that the same deposit
	// operation is not executed multiple times.
	Deposit(userID string, amount float64, idempotencyKey string) error

	// Withdraw subtracts the specified amount from the user's wallet balance.
	// The idempotencyKey parameter is used to ensure that the same withdrawal
	// operation is not executed multiple times.
	Withdraw(userID string, amount float64, idempotencyKey string) error

	// GetTransactions retrieves all the transactions associated with the
	// specified user's wallet.
	GetTransactions(userID string, page, perPage int) (models.PaginatedData[models.Transaction], error)

	// GetAmount retrieves the current balance of the specified user's wallet.
	GetAmount(userID string) (float64, error)

	// RemoveExpiredIdempotencies removes any expired idempotency history from the user's wallet.
	RemoveExpiredIdempotencies(createdAt time.Time) error

	// GetTransactionsReport generates a report of transactions for a specified year.
	// It returns a map where the key is the year of transaction
	// and the value is the sum amount of transactions for that year, represented as decimal.Decimal.
	GetTransactionsReport(yearNumber int) (map[int]decimal.Decimal, error)
}
type WalletAppService struct {
	Repository repositories.WalletRepository
}

func NewWalletService(repo repositories.WalletRepository) *WalletAppService {
	return &WalletAppService{Repository: repo}
}
func (w *WalletAppService) Deposit(userID string, amount float64, idempotencyKey string) error {
	if amount <= 0 {
		return fmt.Errorf("deposit amount must be greater than zero")
	}
	if userID == "" {
		return fmt.Errorf("user ID cannot be empty")
	}
	if idempotencyKey == "" {
		return fmt.Errorf("idempotency key cannot be empty")
	}
	return w.Repository.Deposit(userID, amount, idempotencyKey)
}

func (w *WalletAppService) Withdraw(userID string, amount float64, idempotencyKey string) error {
	if amount <= 0 {
		return fmt.Errorf("withdraw amount must be greater than zero")
	}
	if userID == "" {
		return fmt.Errorf("user ID cannot be empty")
	}
	if idempotencyKey == "" {
		return fmt.Errorf("idempotency key cannot be empty")
	}

	currentAmount, err := w.Repository.GetAmount(userID)
	if err != nil {
		return fmt.Errorf("error retrieving current amount: %w", err)
	}
	if currentAmount < amount {
		return fmt.Errorf("insufficient funds")
	}

	return w.Repository.Withdraw(userID, amount, idempotencyKey)
}

func (w *WalletAppService) GetTransactions(userID string, page, perPage int) (models.PaginatedData[models.Transaction], error) {
	if userID == "" {
		return models.PaginatedData[models.Transaction]{},
			fmt.Errorf("user ID cannot be empty")
	}

	if page < 1 {
		page = 1
	}
	if perPage < 1 {
		perPage = 10
	}

	return w.Repository.GetTransactions(userID, page, perPage)
}

func (w *WalletAppService) GetAmount(userID string) (float64, error) {
	if userID == "" {
		return 0, fmt.Errorf("user ID cannot be empty")
	}
	return w.Repository.GetAmount(userID)
}

func (w *WalletAppService) RemoveExpiredIdempotencies(createdAt time.Time) error {
	if createdAt.IsZero() {
		return fmt.Errorf("createdAt cannot be zero")
	}
	c := cron.New()
	err := c.AddFunc("@every 1h", func() {
		if err := w.Repository.RemoveExpire(time.Now().Add(-1 * time.Hour)); err != nil {
			log.Println(err.Error())
		}
	})

	c.Start()
	return err
}

func (w *WalletAppService) GetTransactionsReport(yearNumber int) (map[int]decimal.Decimal, error) {
	if yearNumber <= 0 {
		return nil, fmt.Errorf("year number must be greater than zero")
	}
	return w.Repository.GetTransactionsReport(yearNumber)
}
