package services

import (
	"errors"
	"testing"
	"time"
	"wallet-api/internal/models"
	"wallet-api/internal/repositories"

	"github.com/google/uuid"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/mock"
	"google.golang.org/genproto/googleapis/type/decimal"
)

// MockWalletRepository is a mock implementation of WalletRepository
type MockWalletRepository struct {
	mock.Mock
}

func (m *MockWalletRepository) Deposit(userID string, amount float64, idempotencyKey string) error {
	args := m.Called(userID, amount, idempotencyKey)
	return args.Error(0)
}

func (m *MockWalletRepository) Withdrawal(userID string, amount float64, idempotencyKey string) error {
	args := m.Called(userID, amount, idempotencyKey)
	return args.Error(0)
}

func (m *MockWalletRepository) GetTransactions(userID string, page, perPage int) (models.PaginatedData[models.Transaction], error) {
	args := m.Called(userID, page, perPage)
	return args.Get(0).(models.PaginatedData[models.Transaction]), args.Error(1)
}

func (m *MockWalletRepository) GetAmount(userID string) (float64, error) {
	args := m.Called(userID)
	return args.Get(0).(float64), args.Error(1)
}

func (m *MockWalletRepository) RemoveExpire(expireTime time.Time) error {
	args := m.Called(expireTime)
	return args.Error(0)
}

func (m *MockWalletRepository) GetTransactionsReport(yearNumber int) (map[int]decimal.Decimal, error) {
	args := m.Called(yearNumber)
	return args.Get(0).(map[int]decimal.Decimal), args.Error(1)
}

func TestWalletService_Deposit(t *testing.T) {
	mockRepo := new(MockWalletRepository)
	service := NewWalletService(mockRepo)

	tests := []struct {
		name           string
		userID         string
		amount         float64
		idempotencyKey string
		mockSetup      func()
		expectedError  error
	}{
		{
			name:           "successful deposit",
			userID:         "user123",
			amount:         100.0,
			idempotencyKey: uuid.New().String(),
			mockSetup: func() {
				mockRepo.On("Deposit", "user123", 100.0, mock.AnythingOfType("string")).Return(nil)
			},
			expectedError: nil,
		},
		{
			name:           "invalid amount",
			userID:         "user123",
			amount:         0,
			idempotencyKey: uuid.New().String(),
			mockSetup:      func() {},
			expectedError:  errors.New("deposit amount must be greater than zero"),
		},
		{
			name:           "empty user ID",
			userID:         "",
			amount:         100.0,
			idempotencyKey: uuid.New().String(),
			mockSetup:      func() {},
			expectedError:  errors.New("user ID cannot be empty"),
		},
		{
			name:           "empty idempotency key",
			userID:         "user123",
			amount:         100.0,
			idempotencyKey: "",
			mockSetup:      func() {},
			expectedError:  errors.New("idempotency key cannot be empty"),
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			tt.mockSetup()
			err := service.Deposit(tt.userID, tt.amount, tt.idempotencyKey)
			if tt.expectedError != nil {
				assert.EqualError(t, err, tt.expectedError.Error())
				return
			}
			assert.NoError(t, err)
			mockRepo.AssertExpectations(t)
		})
	}
}

func TestWalletService_Withdrawal(t *testing.T) {
	mockRepo := new(MockWalletRepository)
	service := NewWalletService(mockRepo)

	tests := []struct {
		name           string
		userID         string
		amount         float64
		idempotencyKey string
		currentAmount  float64
		mockSetup      func()
		expectedError  error
	}{
		{
			name:           "successful withdrawal",
			userID:         "user123",
			amount:         50.0,
			idempotencyKey: uuid.New().String(),
			currentAmount:  100.0,
			mockSetup: func() {
				mockRepo.On("GetAmount", "user123").Return(100.0, nil)
				mockRepo.On("Withdrawal", "user123", 50.0, mock.AnythingOfType("string")).Return(nil)
			},
			expectedError: nil,
		},
		{
			name:           "insufficient funds",
			userID:         "user123",
			amount:         150.0,
			idempotencyKey: uuid.New().String(),
			currentAmount:  100.0,
			mockSetup: func() {
				mockRepo.On("GetAmount", "user123").Return(100.0, nil)
			},
			expectedError: errors.New("insufficient funds"),
		},
		{
			name:           "invalid amount",
			userID:         "user123",
			amount:         0,
			idempotencyKey: uuid.New().String(),
			currentAmount:  100.0,
			mockSetup:      func() {},
			expectedError:  errors.New("withdrawal amount must be greater than zero"),
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			tt.mockSetup()
			err := service.Withdrawal(tt.userID, tt.amount, tt.idempotencyKey)
			if tt.expectedError != nil {
				assert.EqualError(t, err, tt.expectedError.Error())
				return
			}
			assert.NoError(t, err)
			mockRepo.AssertExpectations(t)
		})
	}
}

func TestWalletService_GetTransactions(t *testing.T) {
	mockRepo := new(MockWalletRepository)
	service := NewWalletService(mockRepo)

	tests := []struct {
		name           string
		userID         string
		page           int
		perPage        int
		mockSetup      func()
		expectedError  error
	}{
		{
			name:    "successful retrieval",
			userID:  "user123",
			page:    1,
			perPage: 10,
			mockSetup: func() {
				mockRepo.On("GetTransactions", "user123", 1, 10).Return(
					models.PaginatedData[models.Transaction]{
						Data:       []models.Transaction{},
						Total:      0,
						Page:       1,
						PerPage:    10,
						TotalPages: 0,
					}, nil)
			},
			expectedError: nil,
		},
		{
			name:          "empty user ID",
			userID:        "",
			page:          1,
			perPage:       10,
			mockSetup:     func() {},
			expectedError: errors.New("user ID cannot be empty"),
		},
		{
			name:    "invalid page number",
			userID:  "user123",
			page:    0,
			perPage: 10,
			mockSetup: func() {
				mockRepo.On("GetTransactions", "user123", 1, 10).Return(
					models.PaginatedData[models.Transaction]{
						Data:       []models.Transaction{},
						Total:      0,
						Page:       1,
						PerPage:    10,
						TotalPages: 0,
					}, nil)
			},
			expectedError: nil,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			tt.mockSetup()
			result, err := service.GetTransactions(tt.userID, tt.page, tt.perPage)
			if tt.expectedError != nil {
				assert.EqualError(t, err, tt.expectedError.Error())
				return
			}
			assert.NoError(t, err)
			assert.NotNil(t, result)
			mockRepo.AssertExpectations(t)
		})
	}
}

func TestWalletService_GetAmount(t *testing.T) {
	mockRepo := new(MockWalletRepository)
	service := NewWalletService(mockRepo)

	tests := []struct {
		name           string
		userID         string
		mockSetup      func()
		expectedAmount float64
		expectedError  error
	}{
		{
			name:   "successful retrieval",
			userID: "user123",
			mockSetup: func() {
				mockRepo.On("GetAmount", "user123").Return(100.0, nil)
			},
			expectedAmount: 100.0,
			expectedError:  nil,
		},
		{
			name:           "empty user ID",
			userID:         "",
			mockSetup:      func() {},
			expectedAmount: 0,
			expectedError:  errors.New("user ID cannot be empty"),
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			tt.mockSetup()
			amount, err := service.GetAmount(tt.userID)
			if tt.expectedError != nil {
				assert.EqualError(t, err, tt.expectedError.Error())
				return
			}
			assert.NoError(t, err)
			assert.Equal(t, tt.expectedAmount, amount)
			mockRepo.AssertExpectations(t)
		})
	}
}

func TestWalletService_GetTransactionsReport(t *testing.T) {
	mockRepo := new(MockWalletRepository)
	service := NewWalletService(mockRepo)

	tests := []struct {
		name           string
		yearNumber     int
		mockSetup      func()
		expectedReport map[int]decimal.Decimal
		expectedError  error
	}{
		{
			name:       "successful report generation",
			yearNumber: 2024,
			mockSetup: func() {
				expectedReport := map[int]decimal.Decimal{
					2024: decimal.Decimal{},
				}
				mockRepo.On("GetTransactionsReport", 2024).Return(expectedReport, nil)
			},
			expectedReport: map[int]decimal.Decimal{
				2024: decimal.Decimal{},
			},
			expectedError: nil,
		},
		{
			name:           "invalid year",
			yearNumber:     0,
			mockSetup:      func() {},
			expectedReport: nil,
			expectedError:  errors.New("year number must be greater than zero"),
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			tt.mockSetup()
			report, err := service.GetTransactionsReport(tt.yearNumber)
			if tt.expectedError != nil {
				assert.EqualError(t, err, tt.expectedError.Error())
				return
			}
			assert.NoError(t, err)
			assert.Equal(t, tt.expectedReport, report)
			mockRepo.AssertExpectations(t)
		})
	}
} 