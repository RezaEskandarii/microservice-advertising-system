package http

import (
	"encoding/json"
	"net/http"
	"strings"
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
	http.HandleFunc("/api/v1/transactions", h.getTransactions)
	http.HandleFunc("/api/v1/balance", h.getBalance)
}

type depositRequest struct {
	Amount float64
}

func (h *WalletHandler) deposit(w http.ResponseWriter, r *http.Request) {
	setJsonContentType(w)
	if strings.ToUpper(r.Method) != "POST" {
		http.Error(w, "MethodNotAllowed", http.StatusMethodNotAllowed)
		return
	}
	userID, err := GetUserId(r)
	if err != nil {
		http.Error(w, err.Error(), http.StatusForbidden)
	}
	var req depositRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	idempotencyKey := r.Header.Get("X-Idempotency-Key")

	if err := h.Service.Deposit(userID, req.Amount, idempotencyKey); err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)

	amount, _ := h.Service.GetAmount(userID)

	if err := json.NewEncoder(w).Encode(map[string]float64{"balance": amount}); err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
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

	response := ApiResponse{Data: transactions}
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

	data := ApiResponse{
		Data: map[string]float64{"balance": amount},
	}
	if err := json.NewEncoder(w).Encode(data); err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
}
