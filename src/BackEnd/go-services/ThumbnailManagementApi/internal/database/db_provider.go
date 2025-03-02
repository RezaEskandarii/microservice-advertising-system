package database

import (
	"context"
	"database/sql"
	"github.com/RezaEskandarii/ad-go-commons/env_manager"
	"log"
)

// GetDb retrieves a connection to the database using the provided context.
func GetDb(ctx context.Context) *sql.DB {
	sdn := env_manager.GetString("thumbnail_management_full_db")

	// Open a connection to the PostgreSQL database using the connection string.
	db, err := sql.Open("postgres", sdn)
	if err != nil {
		log.Fatal(err)
	}

	return db
}
