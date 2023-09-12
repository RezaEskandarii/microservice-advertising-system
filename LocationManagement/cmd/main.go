package main

import (
	"location-management/cmd/application"
	"location-management/pkg/env_manager"
	"log"
)

func main() {
	port := env_manager.GetFromDotENV("location_app_port")

	if err := application.Run(port); err != nil {
		log.Fatal(err.Error())
	}
}
