package transaction_types

// TransactionType represents the type of a transaction.
type TransactionType string

// Define transaction types as constants
const (
	Deposit  TransactionType = "deposit"
	Withdraw TransactionType = "withdraw"
	Transfer TransactionType = "transfer"
)
