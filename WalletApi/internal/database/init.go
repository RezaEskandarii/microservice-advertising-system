package database

import (
	"context"
	"database/sql"
	"fmt"
	"log"
)

const (
	DBName = "wallet_api"
)

func Init(ctx context.Context) error {

	// Create the wallet database if not exists
	err := createDBIfNotExists(ctx)
	if err != nil {
		log.Fatal(err)
		return err
	}
	log.Println("wallet database created or already exists")

	db, err := GetWalletApiDB(ctx)
	if err != nil {
		return err
	}

	defer db.Close()

	if err = createWalletsTable(err, db); err != nil {
		return err
	}

	if err = createTransactionsTable(err, db); err != nil {
		return err
	}

	if err = createIdempotencyTable(err, db); err != nil {
		return err
	}

	return nil
}

func createIdempotencyTable(err error, db *sql.DB) error {
	_, err = db.Exec(`
	CREATE TABLE IF NOT EXISTS idempotent_history (
		id SERIAL PRIMARY KEY,
		user_id VARCHAR(255),
		idempotent_key VARCHAR(80) UNIQUE,
		created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
	)`)
	if err != nil {
		log.Fatal(err)
		return err
	}
	log.Println("transactions table created or already exists")
	return err
}

func createTransactionsTable(err error, db *sql.DB) error {
	_, err = db.Exec(`
	CREATE TABLE IF NOT EXISTS transactions (
		id SERIAL PRIMARY KEY,
		user_id VARCHAR(255),
		amount DECIMAL,
		type VARCHAR(50),
		created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
	)`)
	if err != nil {
		log.Fatal(err)
	}
	return err
}

func createWalletsTable(err error, db *sql.DB) error {
	_, err = db.Exec(`
	CREATE TABLE IF NOT EXISTS wallets (
		id SERIAL PRIMARY KEY,
		balance DECIMAL,
		user_id VARCHAR(255) UNIQUE,
	    created_at TIMESTAMP,
	    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
	)`)
	if err != nil {
		log.Fatal(err)
	}
	log.Println("wallets table created or already exists")
	return err
}

func createDBIfNotExists(ctx context.Context) error {
	var exists bool
	db, err := GetPostgresDB(ctx)
	err = db.QueryRow("SELECT 1 FROM pg_database WHERE datname = $1", DBName).Scan(&exists)
	if err != nil && err != sql.ErrNoRows {
		log.Fatalf("Failed to check if database exists: %v", err)
	}

	// If the database does not exist, create it
	if !exists {
		_, err = db.Exec(fmt.Sprintf("CREATE DATABASE %s;", DBName))
		if err != nil {
			log.Fatalf("Failed to create database: %v", err)
			return err
		}
		fmt.Printf("Database %s created successfully.\n", DBName)
	} else {
		fmt.Printf("Database %s already exists.\n", DBName)
	}
	return err
}
