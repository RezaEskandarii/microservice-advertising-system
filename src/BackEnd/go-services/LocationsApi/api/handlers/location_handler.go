package handlers

import (
	"encoding/json"
	. "location-management/api"
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
		writeJSONResponse(w, http.StatusInternalServerError, &ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Failed to retrieve locations",
			Error: &Error{
				Type:   "DatabaseError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		})
		return
	}

	dataLength := len(locations)
	writeJSONResponse(w, http.StatusOK, &ApiResponse{
		Status:  http.StatusOK,
		Message: "Locations retrieved successfully",
		Data:    locations,
		Pagination: &Pagination{
			Page:       1,
			PerPage:    dataLength,
			PagesTotal: dataLength,
		},
	})
}

func writeJSONResponse(w http.ResponseWriter, status int, data *ApiResponse) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)
	json.NewEncoder(w).Encode(data)
}
