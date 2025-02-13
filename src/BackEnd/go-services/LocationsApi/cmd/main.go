package main

import (
	"location-management/cmd/application"
	"location-management/pkg/env_manager"
	"log"
)

func main() {
	//Get the port number from the environment variable
	port := env_manager.GetFromDotENV("location_app_port")

	//Run the application
	if err := application.Run(port); err != nil {
		log.Fatal(err.Error()) // Log the error and exit the program
	}
}
