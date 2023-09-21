package database

import (
	"context"
	"database/sql"
	"log"
	"thumbnail-management/pkg/env_manager"
)

func GetDb(ctx context.Context) *sql.DB {

	sdn := env_manager.Load("thumbnail_management_full_db")

	db, err := sql.Open("postgres", sdn)
	if err != nil {
		log.Fatal(err)
	}
	return db
}
