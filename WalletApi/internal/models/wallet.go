package models

type Wallet struct {
	ID      uint64  `json:"id"`
	Balance float64 `json:"balance"`
	UserID  string  `json:"user_id"`
}

// Transaction represents a wallet transaction.
type Transaction struct {
	ID     uint64  `json:"id"`
	UserID string  `json:"user_id"`
	Amount float64 `json:"amount"`
	Type   string  `json:"type"`
}
