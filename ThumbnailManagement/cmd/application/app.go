package application

import (
	"database/sql"
	"fmt"
	"log"
	"thumbnail-management/grpc"
	"thumbnail-management/pkg/env_manager"
)

type App struct {
}

func New() *App {
	return &App{}
}

func (a App) Run(portNumber int64) {

	dbName := "advertisement_thumbnails"

	baseSdn := env_manager.Load("thumbnail_management_db")
	fullSdn := env_manager.Load("thumbnail_management_db")

	a.createDatabase(dbName, baseSdn)

	// Connect to PostgreSQL
	db, err := sql.Open("postgres", fullSdn)
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	a.createThumbnailsTable(db)

	fs := grpc.NewFileServer()

	fs.Start(fmt.Sprintf("%d", portNumber))

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
			id SERIAL PRIMARY KEY  ,
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
