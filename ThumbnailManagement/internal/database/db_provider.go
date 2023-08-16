package database

import (
	"context"
	"database/sql"
	"fmt"
	"log"
	"thumbnail-management/pkg/secret_manager"
)

var (
	secretManager *secret_manager.SecretManager
)

func init() {
	secretManager = secret_manager.New()
}

func GetDb(ctx context.Context) *sql.DB {

	db, err := sql.Open("postgres", fmt.Sprintf("%s", secretManager.GetConnectionString(ctx, "advertisement_thumbnails")))
	if err != nil {
		log.Fatal(err)
	}
	return db
}
