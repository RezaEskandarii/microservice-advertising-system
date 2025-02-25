package models

import "math"

type ApiResponse[T any] struct {
	Status     int            `json:"status"`
	Message    string         `json:"message"`
	Data       interface{}    `json:"data,omitempty"`
	Pagination *Pagination[T] `json:"pagination,omitempty"`
	Error      *Error         `json:"error"`
}

type Error struct {
	Type     string `json:"type"`
	Title    string `json:"title"`
	Status   int    `json:"status"`
	Detail   string `json:"detail,omitempty"`
	Instance string `json:"instance,omitempty"`
}

type Pagination[T any] struct {
	Page       int `json:"page"`
	PerPage    int `json:"per_page"`
	PagesTotal int `json:"pages_total"`
	TotalItems int `json:"total_items"`
	Data       []T `json:"-"`
}

func PaginateData[T any](items []T, totalItems, page, perPage int) Pagination[T] {
	if page < 1 {
		page = 1
	}
	if perPage < 1 {
		perPage = 10
	}

	totalPages := int(math.Ceil(float64(totalItems) / float64(perPage)))
	if totalPages == 0 {
		totalPages = 1
	}

	return Pagination[T]{
		Page:       page,
		PerPage:    perPage,
		PagesTotal: totalPages,
		TotalItems: totalItems,
		Data:       items,
	}
}
