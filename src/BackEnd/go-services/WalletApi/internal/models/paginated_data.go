package models

import "math"

type PaginatedData[T any] struct {
	Page       int `json:"page"`
	PerPage    int `json:"per_page"`
	PagesTotal int `json:"pages_total"`
	TotalItems int `json:"total_items"`
	Data       []T `json:"-"`
}

func PaginateData[T any](items []T, totalItems, page, perPage int) PaginatedData[T] {
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

	return PaginatedData[T]{
		Page:       page,
		PerPage:    perPage,
		PagesTotal: totalPages,
		TotalItems: totalItems,
		Data:       items,
	}
}
