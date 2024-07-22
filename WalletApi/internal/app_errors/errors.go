package app_errors

import "errors"

var (
	DuplicatedRequestError         = errors.New("duplicate request")
	InsufficientWalletBalanceError = errors.New("wallet balance is insufficient")
	ZeroValueError                 = errors.New("value should not be zero")
	InsufficientFundsError         = errors.New("insufficient funds for transaction")
	InvalidCurrencyError           = errors.New("invalid currency specified")
	NegativeAmountError            = errors.New("negative amount not allowed")
	ExcessiveTransferAmount        = errors.New("transfer amount exceeds daily limit")
	OverdraftLimit                 = errors.New("overdraft limit exceeded")
	FraudDetectedError             = errors.New("potential fraud detected, transaction declined")
	AccountFrozenError             = errors.New("account is frozen, transactions not allowed")
	InvalidIBANError               = errors.New("invalid IBAN provided")
	ExchangeRateUnavailable        = errors.New("exchange rate unavailable for conversion")
	MaxBalanceExceededError        = errors.New("transaction would exceed maximum account balance")
	MinTransactionAmountError      = errors.New("amount below minimum transaction limit")
	UnauthorizedWithdrawal         = errors.New("unauthorized withdrawal attempt")
	InsufficientCollateral         = errors.New("insufficient collateral for loan")
	CreditLimitExceededError       = errors.New("credit limit exceeded")
	PaymentBouncedError            = errors.New("payment bounced due to insufficient funds")
	InvalidAccountTypeError        = errors.New("invalid account type for this transaction")
	TransactionFeesError           = errors.New("unable to cover transaction fees")
	LoanApprovalDeniedError        = errors.New("loan approval denied due to credit score")
	InvalidTaxIDError              = errors.New("invalid tax identification number")
	UnsupportedCryptoError         = errors.New("unsupported cryptocurrency")
)
