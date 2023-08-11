package models

type Category struct {
	ID            int
	Name          string
	Subcategories []Category
}
