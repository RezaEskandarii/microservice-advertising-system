package queue_manager

import (
	"category-management/pkg/email_sender"
	"category-management/pkg/secret_manager"
	"fmt"
	"github.com/streadway/amqp"
	"log"
)

type QueueManager struct {
	SecretManger *secret_manager.SecretManager
	EmailSender  email_sender.EmailSender
}

func New(sm *secret_manager.SecretManager, em email_sender.EmailSender) QueueManager {
	return QueueManager{
		SecretManger: sm,
		EmailSender:  em,
	}
}

func (q QueueManager) Listen() {

	fmt.Println("start to listen to events")

	// RabbitMQ connection string
	connString := "amqp://guest:guest@localhost:5672/"

	// Connect to RabbitMQ
	conn, err := amqp.Dial(connString)
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
		// Process the message
		body := string(msg.Body)
		log.Printf("Received message: %s", body)

		// Check if the event is "OnAdvertisementAdded"
		if msg.RoutingKey == "ad_events.OnAdvertisementAdded" {

			request := email_sender.SendEmailRequest{
				Subject:    "",
				Body:       body,
				Attachment: nil,
			}
			// Send email
			err := q.EmailSender.Send(request)
			if err != nil {
				return
			}
		}
	}

}
