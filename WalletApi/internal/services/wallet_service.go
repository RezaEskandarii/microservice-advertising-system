package services

import (
	"wallet-api/internal/models"
	"wallet-api/internal/repositories"
)

type WalletService interface {
	Deposit(userID string, amount float64, idempotencyKey string) error
	Withdraw(userID string, amount float64, idempotencyKey string) error
	GetTransactions(userID string) ([]models.Transaction, error)
	GetAmount(userID string) (float64, error)
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
