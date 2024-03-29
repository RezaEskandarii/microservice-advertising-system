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
	http.HandleFunc("/deposit", h.Deposit)
	http.HandleFunc("/transactions", h.GetTransactions)
	http.HandleFunc("/amount", h.GetAmount)
}

type depositRequest struct {
	Amount         float64
	IdempotencyKey string
}

func (h *WalletHandler) Deposit(w http.ResponseWriter, r *http.Request) {
	if strings.ToUpper(r.Method) != "POST" {
		http.Error(w, "MethodNotAllowed", http.StatusMethodNotAllowed)
		return
	}
	userID, err := GetUserId(r)
	if err != nil {
		http.Error(w, "user id is required", http.StatusForbidden)
	}
	var req depositRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	if err := h.Service.Deposit(userID, req.Amount, req.IdempotencyKey); err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
}

func (h *WalletHandler) GetTransactions(w http.ResponseWriter, r *http.Request) {
	userID, err := GetUserId(r)
	if err != nil {
		http.Error(w, "user id is required", http.StatusForbidden)
	}

	transactions, err := h.Service.GetTransactions(userID)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	setJsonContentType(w)
	if err := json.NewEncoder(w).Encode(transactions); err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
}

func (h *WalletHandler) GetAmount(w http.ResponseWriter, r *http.Request) {
	userID, err := GetUserId(r)
	if err != nil {
		http.Error(w, "user id is required", http.StatusForbidden)
	}

	amount, err := h.Service.GetAmount(userID)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	setJsonContentType(w)
	if err := json.NewEncoder(w).Encode(map[string]float64{"amount": amount}); err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
}

func setJsonContentType(w http.ResponseWriter) {
	w.Header().Set("Content-Type", "application/json")
}
