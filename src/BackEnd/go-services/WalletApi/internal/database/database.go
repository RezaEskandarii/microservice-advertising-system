package database

import (
	"context"
	"database/sql"
	_ "github.com/lib/pq"
	"wallet-api/pkg/env_manager"
)

// GetPostgresDB retrieves a connection to the postgres base database using the provided context.
func GetPostgresDB(ctx context.Context) (*sql.DB, error) {
	cn := env_manager.LoadEnv("postgres_connection_string")
	return getDB(cn)
}

// GetWalletApiDB retrieves a connection to the wallet api database using the provided context.
func GetWalletApiDB(ctx context.Context) (*sql.DB, error) {
	cn := env_manager.LoadEnv("wallet_api_connection_string")
	return getDB(cn)
}

func getDB(dbURL string) (*sql.DB, error) {
	// Open a connection to the PostgreSQL database using the connection string.
	db, err := sql.Open("postgres", dbURL)
	if err != nil {
		return nil, err
	}

	db.SetMaxOpenConns(50)
	db.SetMaxIdleConns(25)
	db.SetConnMaxLifetime(300)

	return db, nil
}
