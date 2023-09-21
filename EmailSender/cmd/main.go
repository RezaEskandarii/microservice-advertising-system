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

		app.Run(port)
	}

	// Listen for a termination signal
	quit := make(chan os.Signal, 1)
	// kill (no param) default send syscall.SIGTERM
	// kill -2 is syscall.SIGINT
	// kill -9 is syscall. SIGKILL but can"t be catch, so don't need add it
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	log.Println("Shutdown Server ...")
}
