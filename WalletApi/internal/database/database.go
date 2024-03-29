package database

import (
	"context"
	"database/sql"
	_ "github.com/lib/pq"
	"wallet-api/pkg/env_manager"
)

// GetPostgresDB retrieves a connection to the postgres base database using the provided context.
func GetPostgresDB(ctx context.Context) (*sql.DB, error) {
	sdn := env_manager.LoadEnv("postgres_connection_string")

	// Open a connection to the PostgreSQL database using the connection string.
	db, err := sql.Open("postgres", sdn)
	if err != nil {
		return nil, err
	}

	return db, nil
}

// GetWalletApiDB retrieves a connection to the wallet api database using the provided context.
func GetWalletApiDB(ctx context.Context) (*sql.DB, error) {
	sdn := env_manager.LoadEnv("wallet_api_connection_string")

	// Open a connection to the PostgreSQL database using the connection string.
	db, err := sql.Open("postgres", sdn)
	if err != nil {
		return nil, err
	}

	return db, nil
}
