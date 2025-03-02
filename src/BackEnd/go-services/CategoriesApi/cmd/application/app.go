package application

import (
	"category-management/api"
	"category-management/config"
	"category-management/internal/repositories"
	"category-management/internal/services"
	"database/sql"
	"fmt"
	"github.com/RezaEskandarii/ad-go-commons/env_manager"
	"github.com/redis/go-redis/v9"
	"log"
	"net/http"
)

type App struct {
}

func New() *App {
	return &App{}
}

func (a App) Run(portNumber string) {

	if err := a.createDatabase(config.DbName); err != nil {
		log.Fatal(err)
	}

	dbURL := env_manager.GetString("categories_db_connection")
	// Connect to PostgreSQL
	db, err := sql.Open("postgres", dbURL)
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	if err := a.createCategoriesTable(db); err != nil {
		log.Fatal(err)
	}

	categoryRepo := repositories.NewCategoryPostgresRepository(db)

	redisClient := redis.NewClient(&redis.Options{
		Addr: env_manager.GetString("redis_server_address"),
	})

	categoryService := services.NewCategoryService(categoryRepo, redisClient)

	if err = categoryService.Seed(); err != nil {
		log.Fatal(err)
	}

	categoryHandler := api.CategoryAPIHandler{}
	categoryHandler.RegisterRoutes(categoryService)

	// Start the HTTP server
	log.Printf("application started at: %s", portNumber)
	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%s", portNumber), nil))

}

func (a App) createDatabase(dbName string) error {

	dbURL := env_manager.GetString("postgres_base_connection")
	// Connect to db
	db, err := sql.Open("postgres", dbURL)
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

func (a App) createCategoriesTable(db *sql.DB) error {
	createTableQuery := `
		CREATE TABLE IF NOT EXISTS categories (
			id SERIAL PRIMARY KEY,
			name VARCHAR(255) NOT NULL,
			parent_id INTEGER REFERENCES categories(id),
		    properties JSON
		)
	`
	log.Println(createTableQuery)

	_, err := db.Exec(createTableQuery)
	if err != nil {
		return err
	}
	return nil
}
