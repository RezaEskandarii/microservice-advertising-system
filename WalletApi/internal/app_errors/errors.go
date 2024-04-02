package app_errors

import "errors"

var (
	DuplicatedRequestError    = errors.New("duplicate request")
	InsufficientWalletBalance = errors.New("wallet balance is insufficient")
)
