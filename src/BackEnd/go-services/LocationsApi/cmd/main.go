package main

import (
	"github.com/RezaEskandarii/ad-go-commons/env_manager"
	"location-management/cmd/application"
	"log"
)

func main() {
	//Get the port number from the environment variable
	port := env_manager.GetString("location_app_port")

	//Run the application
	if err := application.Run(port); err != nil {
		log.Fatal(err.Error()) // Log the error and exit the program
	}
}
