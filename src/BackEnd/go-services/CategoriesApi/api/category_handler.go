package api

import (
	"category-management/internal/models"
	. "category-management/internal/services"
	"encoding/json"
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
	if strings.ToUpper(r.Method) != "POST" {
		http.Error(w, "Method Not Allowed", http.StatusMethodNotAllowed)
		return
	}

	var category models.Category
	err := json.NewDecoder(r.Body).Decode(&category)
	if err != nil {
		response := ApiResponse{
			Status:  http.StatusBadRequest,
			Message: "Invalid request body",
			Error: &Error{
				Type:   "InvalidRequest",
				Title:  "Bad Request",
				Status: http.StatusBadRequest,
			},
		}
		writeJSONResponse(w, http.StatusBadRequest, response)
		return
	}

	createdCategory, err := h.service.Create(&category)
	if err != nil {
		response := ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Error creating category",
			Error: &Error{
				Type:   "DatabaseError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		}
		writeJSONResponse(w, http.StatusInternalServerError, response)
		return
	}

	response := ApiResponse{
		Status:  http.StatusCreated,
		Message: "Category created successfully",
		Data:    createdCategory,
	}

	writeJSONResponse(w, http.StatusCreated, response)
}

func (h *CategoryAPIHandler) UpdateCategoryHandler(w http.ResponseWriter, r *http.Request) {
	if strings.ToUpper(r.Method) != "PUT" {
		http.Error(w, "Method Not Allowed", http.StatusMethodNotAllowed)
		return
	}

	var category models.Category
	err := json.NewDecoder(r.Body).Decode(&category)
	if err != nil {
		response := ApiResponse{
			Status:  http.StatusBadRequest,
			Message: "Invalid request body",
			Error: &Error{
				Type:   "InvalidRequest",
				Title:  "Bad Request",
				Status: http.StatusBadRequest,
			},
		}
		writeJSONResponse(w, http.StatusBadRequest, response)
		return
	}

	id, err := extractCategoryID(r)
	if err != nil {
		writeJSONResponse(w, http.StatusBadRequest, ApiResponse{
			Status:  http.StatusBadRequest,
			Message: "Invalid category ID",
			Error: &Error{
				Type:   "InvalidRequest",
				Title:  "Bad Request",
				Status: http.StatusBadRequest,
			},
		})
		return
	}

	updatedCategory, err := h.service.Update(id, &category)
	if err != nil {
		response := ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Error updating category",
			Error: &Error{
				Type:   "DatabaseError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		}
		writeJSONResponse(w, http.StatusInternalServerError, response)
		return
	}

	response := ApiResponse{
		Status:  http.StatusOK,
		Message: "Category updated successfully",
		Data:    updatedCategory,
	}

	writeJSONResponse(w, http.StatusOK, response)
}

func (h *CategoryAPIHandler) deleteCategoryHandler(w http.ResponseWriter, r *http.Request) {

	if strings.ToUpper(r.Method) != "DELETE" {
		http.Error(w, "Method Not Allowed", http.StatusMethodNotAllowed)
		return
	}

	id, err := extractCategoryID(r)
	if err != nil {
		writeJSONResponse(w, http.StatusBadRequest, ApiResponse{
			Status:  http.StatusBadRequest,
			Message: "Invalid category ID",
			Error: &Error{
				Type:   "InvalidRequest",
				Title:  "Bad Request",
				Status: http.StatusBadRequest,
			},
		})
		return
	}

	err = h.service.DeleteCategory(id)
	if err != nil {
		writeJSONResponse(w, http.StatusInternalServerError, ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Error deleting category",
			Error: &Error{
				Type:   "DatabaseError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		})
		return
	}

	writeJSONResponse(w, http.StatusOK, ApiResponse{
		Status:  http.StatusOK,
		Message: "Category deleted successfully",
	})

}

func (h *CategoryAPIHandler) findByIDHandler(w http.ResponseWriter, r *http.Request) {
	id, err := extractCategoryID(r)
	if err != nil {
		writeJSONResponse(w, http.StatusBadRequest, ApiResponse{
			Status:  http.StatusBadRequest,
			Message: "Invalid category ID",
			Error: &Error{
				Type:   "InvalidRequest",
				Title:  "Bad Request",
				Status: http.StatusBadRequest,
			},
		})
		return
	}

	category, err := h.service.FindByID(id)
	if err != nil {
		response := ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Error retrieving category",
			Error: &Error{
				Type:   "DatabaseError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		}
		writeJSONResponse(w, http.StatusInternalServerError, response)
		return
	}

	response := ApiResponse{
		Status:  http.StatusOK,
		Message: "Category found",
		Data:    category,
	}

	writeJSONResponse(w, http.StatusOK, response)

}

func (h *CategoryAPIHandler) findAllHandler(w http.ResponseWriter, r *http.Request) {
	categories, err := h.service.FindAll()
	if err != nil {
		writeJSONResponse(w, http.StatusInternalServerError, ApiResponse{
			Status:  http.StatusInternalServerError,
			Message: "Error fetching categories",
			Error: &Error{
				Type:   "DatabaseError",
				Title:  "Internal Server Error",
				Status: http.StatusInternalServerError,
			},
		})
		return
	}

	writeJSONResponse(w, http.StatusOK, ApiResponse{
		Status:  http.StatusOK,
		Message: "Categories retrieved successfully",
		Data:    categories,
		Pagination: &Pagination{
			Page:       1,
			PerPage:    len(categories),
			PagesTotal: 1,
		},
	})
}

func writeJSONResponse(w http.ResponseWriter, status int, data ApiResponse) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)
	json.NewEncoder(w).Encode(data)
}

// Helper function to extract category ID from request
func extractCategoryID(r *http.Request) (int, error) {
	// Extract the URL path
	path := r.URL.Path

	// Split the path into segments
	segments := strings.Split(path, "/")

	// Get the last segment which should be the user ID
	idStr := segments[len(segments)-1]

	// Convert to int
	id, err := strconv.Atoi(idStr)
	if err != nil {
		return -1, err
	}

	return id, nil
}
