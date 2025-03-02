package application

import (
	"context"
	"database/sql"
	"fmt"
	"github.com/RezaEskandarii/ad-go-commons/env_manager"
	"log"
	"thumbnail-management/grpc"
	"thumbnail-management/internal/database"
	"thumbnail-management/internal/repositories"
	"thumbnail-management/internal/services"
)

type App struct {
}

func New() *App {
	return &App{}
}

var (
	thumbnailService       *services.ThumbnailService
	objectStorageAddr      = env_manager.GetString("object_storage_address")
	objectStorageAccessKey = env_manager.GetString("object_storage_access_key")
	objectStorageSecretKey = env_manager.GetString("object_storage_secret_key")
)

func (a App) Run(portNumber int64) error {

	dbName := "advertisement_thumbnails"

	postgresURL := env_manager.GetString("postgres_base_connection")
	thumbnailsDBURL := env_manager.GetString("thumbnails_db_connection")

	if err := a.createDatabase(dbName, postgresURL); err != nil {
		return err
	}

	// Connect to PostgreSQL
	db, err := sql.Open("postgres", thumbnailsDBURL)
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	if err := a.createThumbnailsTable(db); err != nil {
		return err
	}

	fs := grpc.NewFileServer()

	// Initialize the storage manager
	sm, err := services.NewStorageManager(objectStorageAddr, objectStorageAccessKey, objectStorageSecretKey)
	if err != nil {
		return err
	}

	// Initialize the database
	db = database.GetDb(context.Background())

	// Create a PostgreSQL repository for thumbnails
	thumbnailRepo := repositories.NewPostgreSQLRepository(db)
	// Create a ThumbnailService with the repository and storage manager
	thumbnailService = services.NewThumbnailService(thumbnailRepo, sm)

	fs.Register(fmt.Sprintf("%d", portNumber), thumbnailService)

	return nil
}

// createDatabase is a method that connects to a PostgreSQL database and creates a new database if it doesn't already exist.
func (a App) createDatabase(dbName string, sdn string) error {
	// Connect to the database using the provided connection string (sdn)
	db, err := sql.Open("postgres", sdn)
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close() // Close the database connection when the function returns

	// Check if the database already exists
	var exists bool
	err = db.QueryRow("SELECT EXISTS (SELECT 1 FROM pg_catalog.pg_database WHERE datname = $1)", dbName).Scan(&exists)
	if err != nil {
		return err
	}

	// If the database does not exist, create it
	if !exists {
		_, err = db.Exec("CREATE DATABASE " + dbName)
		if err != nil {
			return err
		}
	}

	return nil
}

// createThumbnailsTable is a method that creates a new table named "thumbnails" in the database.
// The table has the following columns:
// - id: a serial primary key
// - image_name: a VARCHAR(255) column to store the name of the image
// - image_bucket: a VARCHAR(255) column to store the name of the bucket where the image is stored
// - advertisement_id: an INT column to store the ID of the advertisement associated with the image
func (a App) createThumbnailsTable(db *sql.DB) error {
	createTableQuery := `
    CREATE TABLE IF NOT EXISTS thumbnails (
        id SERIAL PRIMARY KEY,
        image_name VARCHAR(255),
        image_bucket VARCHAR(255),
        advertisement_id INT
    )
    `
	_, err := db.Exec(createTableQuery)
	if err != nil {
		return err
	}
	return nil
}
