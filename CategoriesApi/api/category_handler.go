package api

import (
	"category-management/internal/models"
	. "category-management/internal/services"
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"
	"strings"
)

type CategoryAPIHandler struct {
	service *CategoryService
}

func (h *CategoryAPIHandler) RegisterRoutes(service *CategoryService) {

	h.service = service
	// Set up HTTP routes and corresponding handlers
	http.HandleFunc("/api/v1/categories/delete", h.deleteCategoryHandler)
	http.HandleFunc("/api/v1/categories/", h.findByIDHandler)
	http.HandleFunc("/api/v1/categories/create", h.CreateCategoryHandler)
	http.HandleFunc("/api/v1/categories/update/", h.UpdateCategoryHandler)
	http.HandleFunc("/api/v1/categories", h.findAllHandler)
}

func (h *CategoryAPIHandler) CreateCategoryHandler(w http.ResponseWriter, r *http.Request) {
	if strings.ToUpper(r.Method) == "POST" {
		// Parse the request body
		var category models.Category
		err := json.NewDecoder(r.Body).Decode(&category)
		if err != nil {
			w.WriteHeader(http.StatusBadRequest)
			fmt.Fprintf(w, "Error parsing request body: %v", err)
			return
		}

		// Create the category
		cat, err := h.service.Create(&category)
		if err != nil {
			w.WriteHeader(http.StatusInternalServerError)
			fmt.Fprintf(w, "Error creating category: %v", err)
			return
		}

		// Return success response
		w.WriteHeader(http.StatusCreated)
		fmt.Fprint(w, cat)
	}
}

func (h *CategoryAPIHandler) UpdateCategoryHandler(w http.ResponseWriter, r *http.Request) {
	if strings.ToUpper(r.Method) == "PUT" {
		// Parse the request body
		var category models.Category
		err := json.NewDecoder(r.Body).Decode(&category)
		if err != nil {
			w.WriteHeader(http.StatusBadRequest)
			fmt.Fprintf(w, "Error parsing request body: %v", err)
			return
		}

		id := extractCategoryID(r)

		// Create the category
		cat, err := h.service.Update(id, &category)
		if err != nil {
			w.WriteHeader(http.StatusInternalServerError)
			fmt.Fprintf(w, "Error creating category: %v", err)
			return
		}

		// Return success response
		w.WriteHeader(http.StatusCreated)
		fmt.Fprint(w, cat)
	}
}

func (h *CategoryAPIHandler) deleteCategoryHandler(w http.ResponseWriter, r *http.Request) {
	id := extractCategoryID(r)
	err := h.service.DeleteCategory(id)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
}

func (h *CategoryAPIHandler) findByIDHandler(w http.ResponseWriter, r *http.Request) {
	id := extractCategoryID(r)
	category, err := h.service.FindByID(id)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	// Convert category to JSON and write response
	response, err := json.Marshal(category)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	w.Write(response)
}

func (h *CategoryAPIHandler) findAllHandler(w http.ResponseWriter, r *http.Request) {
	categories, err := h.service.FindAll()
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	// Convert categories to JSON and write response
	response, err := json.Marshal(categories)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	w.Write(response)
}

// Helper function to extract category ID from request
func extractCategoryID(r *http.Request) int {
	// Extract the URL path
	path := r.URL.Path

	// Split the path into segments
	segments := strings.Split(path, "/")

	// Get the last segment which should be the user ID
	idStr := segments[len(segments)-1]

	// Convert to int
	id, err := strconv.Atoi(idStr)
	if err != nil {
		return -1
	}

	return id
}
