package models

type Category struct {
	ID            int        `json:"id"`
	Name          string     `json:"name"`
	ParentID      int        `json:"parent_id,omitempty"`
	Subcategories []Category `json:"subcategories,omitempty"`
	Properties    []Property `json:"properties"`
}
