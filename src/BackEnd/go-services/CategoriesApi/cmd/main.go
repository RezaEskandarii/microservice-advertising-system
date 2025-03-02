package main

import (
	"category-management/cmd/application"
	"github.com/RezaEskandarii/ad-go-commons/env_manager"
	_ "github.com/lib/pq"
	"log"
)

func main() {

	defer func() {
		if err := recover(); err != nil {
			log.Printf("Recovered from panic: %v", err)
		}
	}()

	app := application.New()
	app.Run(env_manager.GetString("port_number"))
}
