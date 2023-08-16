package main

import (
	"context"
	"database/sql"
	"fmt"
	"log"
	"net/http"
	"thumbnail-management/pkg/secret_manager"
)

type App struct {
}

func NewApp() *App {
	return &App{}
}

func (a App) Run(portNumber int) {

	secretManager := secret_manager.New()
	dbName := "advertisement_categories"

	ctx := context.Background()
	sdn, err := secretManager.Get(ctx, "thumbnail_db_full_connection")
	if err != nil {
		panic(err.Error())
	}
	createDBSdn, err := secretManager.Get(ctx, "thumbnail_db_base_connection")
	if err != nil {
		panic(err.Error())
	}
	a.createDatabase(dbName, fmt.Sprintf("%s", createDBSdn))

	// Connect to PostgreSQL
	db, err := sql.Open("postgres", fmt.Sprintf("%s", sdn))
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	a.createThumbnailsTable(db)

	// Start the HTTP server
	log.Printf("application started at: %d", portNumber)
	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%d", portNumber), nil))

}

func (a App) createDatabase(dbName string, sdn string) error {

	// Connect to db
	db, err := sql.Open("postgres", sdn)
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

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

func (a App) createThumbnailsTable(db *sql.DB) error {
	createTableQuery := `
	CREATE TABLE IF NOT EXISTS thumbnails (
			id INT AUTO_INCREMENT PRIMARY KEY,
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
