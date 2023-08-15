package main

import (
	"category-management/pkg/email_sender"
	"category-management/pkg/queue_manager"
	"category-management/pkg/secret_manager"
	"fmt"
	"log"
	"net/http"
)

type App struct {
}

func NewApp() *App {
	return &App{}
}

func (a App) Run(portNumber int) {

	var secretManager = secret_manager.SecretManager{}
	var emailSender = email_sender.EmailSenderImpl{}
	var queueManager = queue_manager.New(&secretManager, emailSender)

	go queueManager.Listen()
	log.Printf("application started at: %d", portNumber)
	// Start the HTTP server
	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%d", portNumber), nil))
}
