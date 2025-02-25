package application

import (
	"database/sql"
	"fmt"
	"github.com/redis/go-redis/v9"
	"location-management/api/handlers"
	"location-management/internal"
	"location-management/pkg/env_manager"
	"log"
	"net/http"
)

// Run initializes and runs the application by setting up the database connection, creating necessary tables,
// seeding initial data, registering routes for handling location-related requests,
// and starting the HTTP server.
func Run(portNumber string) error {
	// Get the db connection string from the environment variable
	sdn := env_manager.GetFromDotENV("location_management_db_connection")

	// Connect to the PostgreSQL database
	db, err := sql.Open("postgres", sdn)
	if err != nil {
		log.Fatal(err)
		return err // Return the error to indicate connection failure
	}
	defer db.Close() // Close the database connection when the function exits

	redisClient := redis.NewClient(&redis.Options{
		Addr: env_manager.GetFromDotENV("redis_server_address"),
	})

	// Create a new LocationService instance to manage location data
	locationService := internal.NewLocationService(db, redisClient)

	// Create the location_management database if it doesn't exist
	if err = locationService.CreateDB(); err != nil {
		return err // Return the error if database creation fails
	}

	// Create the countries and locations tables in the database
	if err = locationService.CreateTables(); err != nil {
		return err // Return the error if table creation fails
	}

	// Seed the database with initial data from locations.json
	if err = locationService.Seed(); err != nil {
		return err
	}

	// Create a LocationHandler instance to handle location-related requests
	locationHandler := handlers.LocationHandler{}

	// Register the location service with the location handler
	locationHandler.RegisterRoutes(locationService)

	log.Printf("application started at: %s", portNumber)

	if err = startServer(portNumber); err != nil {
		log.Fatalf("Failed to start server: %v", err)
	}

	return err
}

// startServer
func startServer(port string) error {
	address := fmt.Sprintf(":%s", port)
	return http.ListenAndServe(address, nil)
}
