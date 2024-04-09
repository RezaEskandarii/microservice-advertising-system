package application

import (
	"category-management/pkg/email_sender"
	"category-management/pkg/queue_manager"
	"fmt"
	"log"
	"net/http"
)

type App struct {
}

func New() *App {
	return &App{}
}

func (a App) Run(portNumber int) {

	defer func() {
		if r := recover(); r != nil {
			log.Println(r)
			return
		}
	}()

	emailSender := email_sender.EmailSenderImpl{}
	queueManager := queue_manager.New(emailSender)

	go queueManager.Listen()

	log.Printf("application started at: %d", portNumber)
	// Start the HTTP server
	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%d", portNumber), nil))
}
