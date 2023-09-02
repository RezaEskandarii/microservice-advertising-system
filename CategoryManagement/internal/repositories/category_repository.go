package repositories

import (
	. "category-management/internal/models"
	"database/sql"
	"encoding/json"
	"io"
	"log"
	"os"
)

type CategoryRepository interface {
	DeleteCategory(id int) error
	FindByID(id int) (*Category, error)
	Create(category *Category) (*Category, error)
	Update(id int, category *Category) (*Category, error)
	FindAll() ([]Category, error)
	Seed() error
}

type CategoryPostgresRepository struct {
	db *sql.DB
}

// NewCategoryPostgresRepository{
func NewCategoryPostgresRepository(db *sql.DB) *CategoryPostgresRepository {
	return &CategoryPostgresRepository{db: db}
}

func (r *CategoryPostgresRepository) Create(category *Category) (*Category, error) {
	query := "INSERT INTO categories (name,parent_id) VALUES ($1,$2) RETURNING id"
	err := r.db.QueryRow(query, category.Name, category.ParentID).Scan(&category.ID)
	if err != nil {
		return nil, err
	}
	return category, nil
}

func (r *CategoryPostgresRepository) Update(id int, category *Category) (*Category, error) {
	query := "UPDATE categories SET name = $1 WHERE id = $2 RETURNING id"
	err := r.db.QueryRow(query, category.Name, id).Scan(&category.ID)
	if err != nil {
		return nil, err
	}
	return category, nil
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
		return nil, handleNoRowsError(err)
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
		return nil, handleNoRowsError(err)
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

// Seed read default categories and subcategories from json file and insert to database
func (r *CategoryPostgresRepository) Seed() error {

	db := r.db
	jsonFile, err := os.Open("./categories.json")
	if err != nil {
		log.Fatal(err)
	}
	defer jsonFile.Close()

	byteValue, _ := io.ReadAll(jsonFile)

	var categories []Category
	err = json.Unmarshal(byteValue, &categories)
	if err != nil {
		return err
	}

	// Insert parent categories and store their IDs
	for _, category := range categories {
		if category.ParentID == 0 {

			if categoryExists(db, category.Name) {
				continue
			}
			result, err := db.Exec("INSERT INTO categories (name, parent_id) VALUES (?, ?)",
				category.Name, nil)
			if err != nil {
				log.Fatal(err)
			}

			parentID, err := result.LastInsertId()
			if err != nil {
				return err
			}

			// Insert child categories with parent IDs
			for _, child := range category.Subcategories {
				if categoryExists(db, child.Name) {
					continue
				}
				_, err := db.Exec("INSERT INTO categories (name, parent_id) VALUES (?, ?)",
					child.Name, parentID)
				if err != nil {
					log.Fatal(err)
				}

			}
		}
	}

	return nil
}

func handleNoRowsError(err error) error {
	if err == sql.ErrNoRows {
		return nil
	}
	return err
}

func categoryExists(db *sql.DB, name string) bool {
	var count int
	err := db.QueryRow("SELECT COUNT(*) FROM categories WHERE name = ?", name).Scan(&count)
	if err != nil {
		log.Fatal(err)
	}

	return count > 0
}
