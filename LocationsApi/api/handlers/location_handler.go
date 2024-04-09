package handlers

import (
	"encoding/json"
	"location-management/internal"
	"net/http"
)

type LocationHandler struct {
	LocationService internal.LocationService
}

func (h *LocationHandler) RegisterRoutes(service internal.LocationService) {
	h.LocationService = service
	http.HandleFunc("/api/v1/locations", h.findAllHandler)
}

func (h *LocationHandler) findAllHandler(w http.ResponseWriter, r *http.Request) {
	locations, err := h.LocationService.GetAll()
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	// Convert locations to JSON and write response
	response, err := json.Marshal(&locations)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	w.Write(response)
}
