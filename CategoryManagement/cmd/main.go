package main

import (
	"category-management/cmd/application"
	"log"
	_ "github.com/lib/pq"
)


func main() {
	
	defer func() {
			if err := recover(); err != nil {
				log.Printf("Recovered from panic: %v", err)
			}
		}()

	app := application.New()
	app.Run(5005)
}
