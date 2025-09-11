package application

import (
	"email-sender/pkg/email_sender"
	"email-sender/pkg/queue_manager"
	"fmt"
	"log"
	"net/http"
)

// App represents the main application structure
type App struct {
}

// New creates and returns a new instance of App
func New() *App {
	return &App{}
}

// Run starts the application on the specified port number
func (a App) Run(portNumber int) {

	defer func() {
		if r := recover(); r != nil {
			log.Println(r)
			return
		}
	}()

	// Initialize the email sender implementation
	emailSender := email_sender.EmailSenderImpl{}

	// Create a new instance of the queue manager, passing the email sender
	queueManager := queue_manager.New(emailSender)

	// Start listening to the queue in a separate goroutine
	go queueManager.Listen()

	log.Printf("application started at: %d", portNumber)

	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%d", portNumber), nil))
}
