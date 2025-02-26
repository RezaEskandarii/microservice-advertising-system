package http

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"wallet-api/internal/services"
)

type WalletHandler struct {
	Service services.WalletService
}

func NewWalletHandler(service services.WalletService) *WalletHandler {
	return &WalletHandler{Service: service}
}

// InitRoutes initialize http routes
func (h *WalletHandler) InitRoutes() {
	http.HandleFunc("/api/v1/deposit", h.deposit)
	http.HandleFunc("/api/v1/withdraw", h.withdraw)
	http.HandleFunc("/api/v1/transactions", h.getTransactions)
	http.HandleFunc("/api/v1/balance", h.getBalance)
}

type depositRequest struct {
	Amount float64
}

type withdrawRequest struct {
	Amount float64
}

func (h *WalletHandler) deposit(w http.ResponseWriter, r *http.Request) {
	setJsonContentType(w)
	fmt.Println(r.Method + "  " + r.RemoteAddr)
	if r.Method != http.MethodPost {
		writeJSONResponse(w, http.StatusMethodNotAllowed, &ApiResponse{
			Status:  http.StatusMethodNotAllowed,
			Message: "Method Not Allowed",
			Error: &Error{
				Type:   "MethodNotAllowed",
				Title:  "Invalid HTTP Method",
				Status: http.StatusMethodNotAllowed,
				Detail: "Only POST requests are allowed",
			},
		})
		return
	}

	userID, err := GetUserId(r)
	if err != nil {
		writeJSONResponse(w, http.StatusForbidden, &ApiResponse{
			Status:  http.StatusForbidden,
			Message: "Unauthorized access",
			Error: &Error{
				Type:   "AuthenticationError",
				Title:  "Unauthorized",
				Status: http.StatusForbidden,
			},
		})
		return
	}

	var req depositRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSONResponse(w, http.StatusBadRequest, &ApiResponse{
			Status:  http.StatusBadRequest,
			Message: "Invalid request body",
			Error: &Error{
				Type:   "BadRequest",
				Title:  "Malformed JSON",
				Status: http.StatusBadRequest,
				Detail: err.Error(),
			},
		})
		return
	}

	idempotencyKey := r.Header.Get("X-Idempotency-Key")

	if err := h.Service.Deposit(userID, req.Amount, idempotencyKey); err != nil {
		log.Println(err)
		writeJSONResponse(w, http.StatusInternalServerError, &ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Deposit failed",
			Error: &Error{
				Type:   "ServerError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		})
		return
	}

	amount, _ := h.Service.GetAmount(userID)

	writeJSONResponse(w, http.StatusOK, &ApiResponse{
		Status:  http.StatusOK,
		Message: "Deposit successful",
		Data:    map[string]float64{"balance": amount},
	})
}

func (h *WalletHandler) withdraw(w http.ResponseWriter, r *http.Request) {
	setJsonContentType(w)

	if r.Method != http.MethodPost {
		writeJSONResponse(w, http.StatusMethodNotAllowed, &ApiResponse{
			Status:  http.StatusMethodNotAllowed,
			Message: "Method Not Allowed",
			Error: &Error{
				Type:   "MethodNotAllowed",
				Title:  "Invalid HTTP Method",
				Status: http.StatusMethodNotAllowed,
				Detail: "Only POST requests are allowed",
			},
		})
		return
	}

	userID, err := GetUserId(r)
	if err != nil {
		writeJSONResponse(w, http.StatusForbidden, &ApiResponse{
			Status:  http.StatusForbidden,
			Message: "Unauthorized access",
			Error: &Error{
				Type:   "AuthenticationError",
				Title:  "Unauthorized",
				Status: http.StatusForbidden,
				Detail: err.Error(),
			},
		})
		return
	}

	var req withdrawRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSONResponse(w, http.StatusBadRequest, &ApiResponse{
			Status:  http.StatusBadRequest,
			Message: "Invalid request body",
			Error: &Error{
				Type:   "BadRequest",
				Title:  "Malformed JSON",
				Status: http.StatusBadRequest,
				Detail: err.Error(),
			},
		})
		return
	}

	idempotencyKey := r.Header.Get("X-Idempotency-Key")

	if err := h.Service.Withdraw(userID, req.Amount, idempotencyKey); err != nil {
		writeJSONResponse(w, http.StatusInternalServerError, &ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Withdrawal failed",
			Error: &Error{
				Type:   "ServerError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		})
		return
	}

	amount, _ := h.Service.GetAmount(userID)

	writeJSONResponse(w, http.StatusOK, &ApiResponse{
		Status:  http.StatusOK,
		Message: "Withdrawal successful",
		Data:    map[string]float64{"balance": amount},
	})
}

func (h *WalletHandler) getTransactions(w http.ResponseWriter, r *http.Request) {
	setJsonContentType(w)

	userID, err := GetUserId(r)
	if err != nil {
		writeJSONResponse(w, http.StatusForbidden, &ApiResponse{
			Status:  http.StatusForbidden,
			Message: "Unauthorized access",
			Error: &Error{
				Type:   "AuthenticationError",
				Title:  "Unauthorized",
				Status: http.StatusForbidden,
				Detail: err.Error(),
			},
		})
		return
	}

	page, perPage := parsePaginationParams(r)
	transactions, err := h.Service.GetTransactions(userID, page, perPage)

	if err != nil {
		writeJSONResponse(w, http.StatusInternalServerError, &ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "get transactions failed",
			Error: &Error{
				Type:   "ServerError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		})
		return
	}

	writeJSONResponse(w, http.StatusOK, &ApiResponse{
		Status:  http.StatusOK,
		Message: "Transactions retrieved successfully",
		Data:    transactions.Data,
		Pagination: &Pagination{
			Page:       transactions.Page,
			PerPage:    transactions.PerPage,
			PagesTotal: transactions.PagesTotal,
			TotalItems: transactions.TotalItems,
		},
	})
}

func (h *WalletHandler) getBalance(w http.ResponseWriter, r *http.Request) {
	setJsonContentType(w)

	userID, err := GetUserId(r)
	if err != nil {
		writeJSONResponse(w, http.StatusForbidden, &ApiResponse{
			Status:  http.StatusForbidden,
			Message: "Unauthorized access",
			Error: &Error{
				Type:   "AuthenticationError",
				Title:  "Unauthorized",
				Status: http.StatusForbidden,
				Detail: err.Error(),
			},
		})
		return
	}

	amount, err := h.Service.GetAmount(userID)
	if err != nil {
		writeJSONResponse(w, http.StatusInternalServerError, &ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Failed to retrieve balance",
			Error: &Error{
				Type:   "ServerError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		})
		return
	}

	writeJSONResponse(w, http.StatusOK, &ApiResponse{
		Status:  http.StatusOK,
		Message: "Balance retrieved successfully",
		Data:    map[string]float64{"balance": amount},
	})
}
