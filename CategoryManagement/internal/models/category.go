package models

type Category struct {
	ID            int
	Name          string `json:"name"`
	ParentID      int
	Subcategories []Category `json:"subcategories"`
}
