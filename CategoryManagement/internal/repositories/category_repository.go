package repositories

import (
	. "category-management/internal/models"
	"database/sql"
)

type CategoryRepository interface {
	CreateTable() error
	DeleteCategory(id int) error
	FindByID(id int) (*Category, error)
	FindAll() ([]Category, error)
}

type CategoryPostgresRepository struct {
	db *sql.DB
}

// NewCategoryPostgresRepository{
func NewCategoryPostgresRepository(db *sql.DB) *CategoryPostgresRepository {
	return &CategoryPostgresRepository{db: db}
}

// CreateTable
func (r *CategoryPostgresRepository) CreateTable() error {
	createTableQuery := `
		CREATE TABLE IF NOT EXISTS categories (
			id SERIAL PRIMARY KEY,
			name VARCHAR(255) NOT NULL,
			parent_id INTEGER REFERENCES categories(id)
		)
	`
	_, err := r.db.Exec(createTableQuery)
	if err != nil {
		return err
	}
	return nil
}

// DeleteCategory
func (r *CategoryPostgresRepository) DeleteCategory(id int) error {
	_, err := r.db.Exec("DELETE FROM categories WHERE id = $1", id)
	if err != nil {
		return err
	}
	return nil
}

// FindByID
func (r *CategoryPostgresRepository) FindByID(id int) (*Category, error) {
	category := &Category{}
	err := r.db.QueryRow("SELECT id, name FROM categories WHERE id = $1", id).Scan(&category.ID, &category.Name)
	if err != nil {
		return nil, err
	}

	subcategories, err := r.findSubcategories(category.ID)
	if err != nil {
		return nil, err
	}
	category.Subcategories = subcategories

	return category, nil
}

// FindAll
func (r *CategoryPostgresRepository) FindAll() ([]Category, error) {
	rows, err := r.db.Query("SELECT id, name FROM categories WHERE parent_id IS NULL")
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	categories := []Category{}
	for rows.Next() {
		category := Category{}
		err := rows.Scan(&category.ID, &category.Name)
		if err != nil {
			return nil, err
		}

		subcategories, err := r.findSubcategories(category.ID)
		if err != nil {
			return nil, err
		}
		category.Subcategories = subcategories

		categories = append(categories, category)
	}

	return categories, nil
}

// findSubcategories
func (r *CategoryPostgresRepository) findSubcategories(parentID int) ([]Category, error) {
	rows, err := r.db.Query("SELECT id, name FROM categories WHERE parent_id = $1", parentID)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	subcategories := []Category{}
	for rows.Next() {
		category := Category{}
		err := rows.Scan(&category.ID, &category.Name)
		if err != nil {
			return nil, err
		}

		subsubcategories, err := r.findSubcategories(category.ID)
		if err != nil {
			return nil, err
		}
		category.Subcategories = subsubcategories

		subcategories = append(subcategories, category)
	}

	return subcategories, nil
}
