package application

import (
	"context"
	"database/sql"
	"fmt"
	"log"
	"thumbnail-management/grpc"
	"thumbnail-management/pkg/secret_manager"
)

type App struct {
}

func New() *App {
	return &App{}
}

func (a App) Run(portNumber int64) {

	secretManager := secret_manager.New()
	dbName := "advertisement_thumbnails"

	ctx := context.Background()

	a.createDatabase(dbName, fmt.Sprintf("%s", secretManager.GetConnectionString(ctx, "")))

	// Connect to PostgreSQL
	db, err := sql.Open("postgres", fmt.Sprintf("%s", secretManager.GetConnectionString(ctx, "advertisement_thumbnails")))
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	a.createThumbnailsTable(db)

	fs := grpc.NewFleServer()

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
