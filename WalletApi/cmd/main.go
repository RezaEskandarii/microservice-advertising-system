package main

import (
	"context"
	"log"
	"wallet-api/cmd/app"
)

func main() {
	ctx := context.Background()

	if err := app.Run(ctx); err != nil {
		log.Fatal(err.Error())
	}
}
