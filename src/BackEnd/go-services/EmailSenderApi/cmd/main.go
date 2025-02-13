package main

import (
	"category-management/cmd/application"
	env "category-management/pkg/env_manager"
	"log"
	"os"
	"os/signal"
	"strconv"
	"syscall"
)

func main() {

	app := application.New()

	portStr := env.Load("app_port")

	if port, err := strconv.Atoi(portStr); err != nil {
		panic(err.Error())
	} else {
		// Start the application on the specified port
		app.Run(port)
	}

	// Create a channel to listen for termination signals (e.g., SIGINT, SIGTERM)
	quit := make(chan os.Signal, 1)

	// Notify the quit channel if an interrupt or termination signal is received
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)

	// Block until a signal is received
	<-quit

	log.Println("Shutdown Server ...")
}
