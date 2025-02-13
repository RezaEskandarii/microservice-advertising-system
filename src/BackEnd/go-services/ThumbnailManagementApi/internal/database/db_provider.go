package database

import (
	"context"
	"database/sql"
	"log"
	"thumbnail-management/pkg/env_manager"
)

// GetDb retrieves a connection to the database using the provided context.
func GetDb(ctx context.Context) *sql.DB {
	sdn := env_manager.Load("thumbnail_management_full_db")

	// Open a connection to the PostgreSQL database using the connection string.
	db, err := sql.Open("postgres", sdn)
	if err != nil {
		log.Fatal(err)
	}

	return db
}
