package repositories

import (
	"category-management/config"
	. "category-management/internal/models"
	"database/sql"
	"encoding/json"
	"fmt"
	"io"
	"log"
	"os"
	"strings"
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

// NewCategoryPostgresRepository creates a new instance of CategoryPostgresRepository
// It takes an *sql.DB as a parameter and returns a pointer to a CategoryPostgresRepository
func NewCategoryPostgresRepository(db *sql.DB) *CategoryPostgresRepository {
	return &CategoryPostgresRepository{db: db}
}

// Create inserts a new category into the database
func (r *CategoryPostgresRepository) Create(category *Category) (*Category, error) {
	query := "INSERT INTO categories (name,parent_id) VALUES ($1,$2) RETURNING id"
	err := r.db.QueryRow(query, category.Name, category.ParentID).Scan(&category.ID)
	if err != nil {
		return nil, err
	}

	return category, nil
}

// Update updates an existing category in the database
// It takes the ID of the category to update and a pointer to a Category as parameters
// It returns the updated category and an error
func (r *CategoryPostgresRepository) Update(id int, category *Category) (*Category, error) {
	query := "UPDATE categories SET name = $1 WHERE id = $2 RETURNING id"
	err := r.db.QueryRow(query, category.Name, id).Scan(&category.ID)
	if err != nil {
		return nil, err
	}
	return category, nil
}

// DeleteCategory deletes a category from the database
func (r *CategoryPostgresRepository) DeleteCategory(id int) error {
	_, err := r.db.Exec("DELETE FROM categories WHERE id = $1", id)
	if err != nil {
		return err
	}
	return nil
}

// FindByID retrieves a category from the database by its ID
func (r *CategoryPostgresRepository) FindByID(id int) (*Category, error) {
	category := &Category{}

	err := r.db.QueryRow(`SELECT "id", "name","properties" FROM categories WHERE id = $1`, id).
		Scan(&category.ID, &category.Name, &category.Properties)

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

// FindByName retrieves a category from the database by its name
func (r *CategoryPostgresRepository) FindByName(name string) (*Category, error) {
	category := &Category{}
	name = strings.TrimSpace(name)
	err := r.db.QueryRow("SELECT id, name FROM categories WHERE name = $1", name).Scan(&category.ID, &category.Name)
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

// FindAll retrieves all top-level categories from the database
// It returns a slice of Category and an error
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

// findSubcategories retrieves all subcategories for a given parent category ID
// It takes the parent category ID as a parameter and returns a slice of Category and an error
func (r *CategoryPostgresRepository) findSubcategories(parentID int) ([]Category, error) {
	rows, err := r.db.Query("SELECT id, name, parent_id, properties FROM categories WHERE parent_id = $1", parentID)
	if err != nil {
		return nil, handleNoRowsError(err)
	}
	defer rows.Close()

	subcategories := []Category{}
	for rows.Next() {
		category := Category{}
		err := rows.Scan(&category.ID, &category.Name, &category.ParentID, &category.Properties)
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

// Seed reads default categories and subcategories from a JSON file and inserts them into the database
// It returns an error if any issues occur during the seeding process
func (r *CategoryPostgresRepository) Seed() error {

	db := r.db
	jsonFile, err := os.Open("./categories_seed_data.json")
	if err != nil {
		log.Fatal(err)
	}
	defer jsonFile.Close()

	byteValue, _ := io.ReadAll(jsonFile)

	type categoryRequest struct {
		Name string `json:"name"`

		Subcategories []struct {
			Name       string `json:"name"`
			Properties []struct {
				Name     string      `json:"name"`
				DataType string      `json:"data_type"`
				Value    interface{} `json:"value"`
			} `json:"properties"`
		}
	}

	var categories struct {
		Categories []categoryRequest `json:"categories"`
	}

	err = json.Unmarshal(byteValue, &categories)
	if err != nil {
		return err
	}

	// Insert parent categories and store their IDs
	for _, category := range categories.Categories {

		var parentID int
		if !categoryExists(db, category.Name) {

			err := db.QueryRow("INSERT INTO categories (name, parent_id) VALUES ($1, $2) RETURNING id", category.Name, nil).Scan(&parentID)
			if err != nil {
				return err
			}
			log.Printf("######### The %s was seeded ##########", category.Name)
		} else {

			parent, _ := r.FindByName(category.Name)
			parentID = parent.ID
		}

		// Insert child categories with parent IDs
		for _, child := range category.Subcategories {
			if categoryExists(db, child.Name) {
				continue
			}
			properties := child.Properties
			propertiesStr, err := json.Marshal(properties)
			if err != nil {
				log.Fatal(err.Error())
			}
			_, err = db.Exec("INSERT INTO categories (name, parent_id,properties) VALUES ($1, $2, $3)", child.Name, parentID, propertiesStr)
			if err != nil {
				return err
			}

			log.Printf("######### The %s child was seeded ##########", child)

		}
	}

	return nil
}

// handleNoRowsError is a helper function that checks if the provided error is a "no rows" error
// and returns a nil error if it is, or the original error if it's not
func handleNoRowsError(err error) error {
	if err == sql.ErrNoRows {
		return nil
	}
	return err
}

// categoryExists checks if a category with the given name already exists in the database
// It takes the database connection and the category name as parameters and returns a boolean
func categoryExists(db *sql.DB, name string) bool {
	var count int = 0
	queryStr := fmt.Sprintf("SELECT COUNT(*) FROM %s WHERE name = $1", config.CategoriesTblName)
	err := db.QueryRow(queryStr, name).Scan(&count)
	if err != nil {
		log.Fatal(err)
	}

	return count > 0
}
