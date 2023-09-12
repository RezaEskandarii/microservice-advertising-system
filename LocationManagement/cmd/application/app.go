package application

import (
	"database/sql"
	"fmt"
	"location-management/api/handlers"
	"location-management/internal"
	"location-management/pkg/env_manager"
	"log"
	"net/http"
)

func Run(portNumber int) error {

	sdn := env_manager.GetFromDotENV("location_management_full_sdn")

	// Connect to db
	db, err := sql.Open("postgres", sdn)
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	locationService := internal.NewLocationService(db)
	if err := locationService.CreateDB(); err != nil {
		return err
	}
	if _, err := locationService.CreateTable(); err != nil {
		return err
	}

	locationService.Seed()

	// Start the HTTP server
	locationHandler := handlers.LocationHandler{}
	locationHandler.RegisterRoutes(locationService)

	log.Printf("application started at: %d", portNumber)
	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%d", portNumber), nil))

	return nil
}
