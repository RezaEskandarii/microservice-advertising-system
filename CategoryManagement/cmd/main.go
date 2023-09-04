package main

import (
	"category-management/cmd/application"
	_ "github.com/lib/pq"
)

func main() {
	app := application.New()
	app.Run(5005)
}
