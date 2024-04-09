package services

import (
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
	GetTransactions(userID string) ([]models.Transaction, error)

	// GetAmount retrieves the current balance of the specified user's wallet.
	GetAmount(userID string) (float64, error)

	// RemoveExpire removes any expired idempotency history from the user's wallet.
	RemoveExpire(createdAt time.Time) error
}

type WalletAppService struct {
	Repository repositories.WalletRepository
}

func NewWalletService(repo repositories.WalletRepository) *WalletAppService {
	return &WalletAppService{Repository: repo}
}

func (w *WalletAppService) Deposit(userID string, amount float64, idempotencyKey string) error {
	return w.Repository.Deposit(userID, amount, idempotencyKey)
}

func (w *WalletAppService) Withdraw(userID string, amount float64, idempotencyKey string) error {
	return w.Repository.Withdraw(userID, amount, idempotencyKey)
}

func (w *WalletAppService) GetTransactions(userID string) ([]models.Transaction, error) {
	return w.Repository.GetTransactions(userID)
}

func (w *WalletAppService) GetAmount(userID string) (float64, error) {
	return w.Repository.GetAmount(userID)
}

func (w *WalletAppService) RemoveExpire(createdAt time.Time) error {
	return w.Repository.RemoveExpire(createdAt)
}
