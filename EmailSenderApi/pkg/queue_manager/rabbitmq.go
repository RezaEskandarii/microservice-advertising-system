package queue_manager

import (
	"category-management/internal/models"
	"category-management/pkg/email_sender"
	"category-management/pkg/env_manager"
	"encoding/json"
	"fmt"
	"github.com/streadway/amqp"
	"log"
	"strconv"
)

type QueueManager struct {
	EmailSender email_sender.EmailSender
}

// New creates a new instance of QueueManager with the provided EmailSender.
//
// Parameters:
//   - em: An implementation of the email_sender.EmailSender interface to be used for sending emails.
//
// Returns:
//   - QueueManager: A new instance of QueueManager initialized with the provided EmailSender.
func New(em email_sender.EmailSender) QueueManager {
	return QueueManager{
		EmailSender: em,
	}
}

// Listen starts listening to events from a RabbitMQ queue.
// It connects to RabbitMQ using the provided address,
// declares an exchange named "ad_events" of type "topic",
// declares a durable queue named "email_queue", binds it to the exchange
// with routing key "ad_events.OnAdvertisementAdded", and starts consuming messages.
// Upon receiving a message, it invokes the sendEmail method of the QueueManager
// for further processing.
func (q *QueueManager) Listen() {

	fmt.Println("start to listen to events")

	// RabbitMQ connection string
	rabbitMqAddr := env_manager.Load("rabbitmq_addr")

	// Connect to RabbitMQ
	conn, err := amqp.Dial(rabbitMqAddr)
	if err != nil {
		log.Printf("Failed to connect to RabbitMQ: %s", err)
		return
	}
	defer conn.Close()

	// Create a channel
	ch, err := conn.Channel()
	if err != nil {
		log.Fatalf("Failed to open a channel: %s", err)
		return
	}
	defer ch.Close()

	// Declare the exchange
	err = ch.ExchangeDeclare(
		"ad_events", // exchange name
		"topic",     // exchange type
		true,        // durable
		false,       // auto-deleted
		false,       // internal
		false,       // no-wait
		nil,         // arguments
	)
	if err != nil {
		log.Fatalf("Failed to declare the exchange: %s", err)
	}

	// Declare the queue
	declaredQueue, err := ch.QueueDeclare(
		"email_queue", // queue name
		true,          // durable
		false,         // auto-deleted
		false,         // exclusive
		false,         // no-wait
		nil,           // arguments
	)
	if err != nil {
		log.Fatalf("Failed to declare the queue: %s", err)
	}

	// Bind the queue to the exchange
	err = ch.QueueBind(
		declaredQueue.Name,               // queue name
		"ad_events.OnAdvertisementAdded", // routing key
		"ad_events",                      // exchange name
		false,                            // no-wait
		nil,                              // arguments
	)
	if err != nil {
		log.Fatalf("Failed to bind the queue: %s", err)
	}

	// Consume messages from the queue
	msgs, err := ch.Consume(
		declaredQueue.Name, // queue name
		"",                 // consumer name
		true,               // auto-ack
		false,              // exclusive
		false,              // no-local
		false,              // no-wait
		nil,                // arguments
	)
	if err != nil {
		log.Fatalf("Failed to register a consumer: %s", err)
	}

	// Start consuming messages

	for msg := range msgs {
		q.sendEmail(msg)
	}

}

// sendEmail sends an email based on the message received from the queue.
// It processes the message to extract relevant information about the advertisement,
// and if the event corresponds to "OnAdvertisementAdded", it sends an email to
// the user who added the advertisement to notify them about its successful registration.
//
// Parameters:
//   - msg: The message received from the queue, typically containing information about
//     the advertisement event.
//
// Behavior:
//  1. Processes the message to extract advertisement details from its body.
//  2. Checks if the event corresponds to "OnAdvertisementAdded".
//  3. If the event matches, constructs an email message with details of the registered ad
//     and sends it to the user who added the advertisement.
//
// Note: This method assumes that the advertisement information can be extracted from
//
//	the message body and that the email sender is configured appropriately.
//	Errors encountered during email sending are logged but not handled within this method.
func (q *QueueManager) sendEmail(msg amqp.Delivery) {
	// Process the message
	body := string(msg.Body)
	log.Printf("Received message: %s", body)

	// Check if the event is "OnAdvertisementAdded"
	if msg.RoutingKey != "ad_events.OnAdvertisementAdded" {
		return
	}

	ad := fetchAdvertisementFromMessageBody(body)
	if ad == nil {
		return
	}

	// Prepare email request
	request := email_sender.SendEmailRequest{
		Subject: "Your ad has been successfully registered.",
		Body:    fmt.Sprintf("Your ad has been successfully registered. Ad title is %s", ad.Title),
		To:      ad.UserEmail,
	}

	// Send email
	if err := q.EmailSender.Send(request); err != nil {
		log.Printf("Error sending email: %v", err)
	}
}

// fetchAdvertisementFromMessageBody parses the JSON-encoded advertisement data
// from the given message body and returns an Advertisement struct pointer.
// If there are any decoding errors, it logs the error and returns nil.
//
// Parameters:
//   - jsonStr: The JSON-encoded string containing advertisement data.
//
// Returns:
//   - *models.Advertisement: A pointer to the Advertisement struct if decoding is successful,
//     otherwise nil.
func fetchAdvertisementFromMessageBody(jsonStr string) *models.Advertisement {
	var result models.Advertisement

	unquotedStr, err := strconv.Unquote(jsonStr)
	if err != nil {
		log.Printf("Error decoding UTF-8 JSON: %s", err.Error())
		return nil
	}

	err = json.Unmarshal([]byte(unquotedStr), &result)
	if err != nil {
		log.Printf("Error decoding JSON: %s", err.Error())
		return nil
	}

	return &result
}
