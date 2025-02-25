package http

import (
	"encoding/json"
	"net/http"
	"wallet-api/internal/models"
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

	if r.Method != http.MethodPost {
		writeJSONResponse(w, http.StatusMethodNotAllowed, &models.ApiResponse{
			Status:  http.StatusMethodNotAllowed,
			Message: "Method Not Allowed",
			Error: &models.Error{
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
		writeJSONResponse(w, http.StatusForbidden, &models.ApiResponse{
			Status:  http.StatusForbidden,
			Message: "Unauthorized access",
			Error: &models.Error{
				Type:   "AuthenticationError",
				Title:  "Unauthorized",
				Status: http.StatusForbidden,
			},
		})
		return
	}

	var req depositRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSONResponse(w, http.StatusBadRequest, &models.ApiResponse{
			Status:  http.StatusBadRequest,
			Message: "Invalid request body",
			Error: &models.Error{
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
		writeJSONResponse(w, http.StatusInternalServerError, &models.ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Deposit failed",
			Error: &models.Error{
				Type:   "ServerError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		})
		return
	}

	amount, _ := h.Service.GetAmount(userID)

	writeJSONResponse(w, http.StatusOK, &models.ApiResponse{
		Status:  http.StatusOK,
		Message: "Deposit successful",
		Data:    map[string]float64{"balance": amount},
	})
}

func (h *WalletHandler) withdraw(w http.ResponseWriter, r *http.Request) {
	setJsonContentType(w)

	if r.Method != http.MethodPost {
		writeJSONResponse(w, http.StatusMethodNotAllowed, &models.ApiResponse{
			Status:  http.StatusMethodNotAllowed,
			Message: "Method Not Allowed",
			Error: &models.Error{
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
		writeJSONResponse(w, http.StatusForbidden, &models.ApiResponse{
			Status:  http.StatusForbidden,
			Message: "Unauthorized access",
			Error: &models.Error{
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
		writeJSONResponse(w, http.StatusBadRequest, &models.ApiResponse{
			Status:  http.StatusBadRequest,
			Message: "Invalid request body",
			Error: &models.Error{
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
		writeJSONResponse(w, http.StatusInternalServerError, &models.ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Withdrawal failed",
			Error: &models.Error{
				Type:   "ServerError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		})
		return
	}

	amount, _ := h.Service.GetAmount(userID)

	writeJSONResponse(w, http.StatusOK, &models.ApiResponse{
		Status:  http.StatusOK,
		Message: "Withdrawal successful",
		Data:    map[string]float64{"balance": amount},
	})
}

func (h *WalletHandler) getTransactions(w http.ResponseWriter, r *http.Request) {
	userID, err := GetUserId(r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusForbidden)
	}

	transactions, err := h.Service.GetTransactions(userID)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	response := models.ApiResponse{Data: transactions}
	setJsonContentType(w)
	if err := json.NewEncoder(w).Encode(response); err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
}

func (h *WalletHandler) getBalance(w http.ResponseWriter, r *http.Request) {
	setJsonContentType(w)
	userID, err := GetUserId(r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusForbidden)
	}

	amount, err := h.Service.GetAmount(userID)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	data := models.ApiResponse{
		Data: map[string]float64{"balance": amount},
	}
	if err := json.NewEncoder(w).Encode(data); err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
}
