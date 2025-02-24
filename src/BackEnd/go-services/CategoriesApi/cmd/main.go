package main

import (
	"category-management/cmd/application"
	"category-management/pkg/env_manager"
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
	app.Run(env_manager.Load("port_number"))
}
