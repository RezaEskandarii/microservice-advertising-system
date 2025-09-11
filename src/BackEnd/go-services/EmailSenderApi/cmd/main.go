package main

import (
	"email-sender/cmd/application"
	env "github.com/RezaEskandarii/ad-go-commons/env_manager"
	"log"
	"os"
	"os/signal"
	"strconv"
	"syscall"
)

func main() {

	app := application.New()

	portStr := env.GetString("app_port")

	if port, err := strconv.Atoi(portStr); err != nil {
		panic(err.Error())
	} else {
		// Start the application on the specified port
		app.Run(port)
	}

	quit := make(chan os.Signal, 1)

	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)

	<-quit

	log.Println("Shutdown Server ...")
}
