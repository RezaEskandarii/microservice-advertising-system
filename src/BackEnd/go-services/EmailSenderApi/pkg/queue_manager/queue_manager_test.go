package queue_manager

import (
	"testing"
	"time"
)

func TestQueueManager_Connect(t *testing.T) {
	tests := []struct {
		name    string
		url     string
		wantErr bool
	}{
		{
			name:    "valid connection",
			url:     "amqp://guest:guest@localhost:5672/",
			wantErr: false,
		},
		{
			name:    "invalid connection",
			url:     "amqp://invalid:invalid@localhost:5672/",
			wantErr: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			qm := NewQueueManager()
			err := qm.Connect(tt.url)
			if (err != nil) != tt.wantErr {
				t.Errorf("QueueManager.Connect() error = %v, wantErr %v", err, tt.wantErr)
			}
			if err == nil {
				defer qm.Close()
			}
		})
	}
}

func TestQueueManager_PublishAndConsume(t *testing.T) {
	if testing.Short() {
		t.Skip("skipping integration test")
	}

	qm := NewQueueManager()
	err := qm.Connect("amqp://guest:guest@localhost:5672/")
	if err != nil {
		t.Fatalf("Failed to connect to RabbitMQ: %v", err)
	}
	defer qm.Close()

	queueName := "test_queue"
	message := "test message"

	// Start consuming in a goroutine
	received := make(chan string)
	go func() {
		err := qm.Consume(queueName, func(msg []byte) error {
			received <- string(msg)
			return nil
		})
		if err != nil {
			t.Errorf("Failed to consume message: %v", err)
		}
	}()

	// Give some time for the consumer to start
	time.Sleep(100 * time.Millisecond)

	// Publish message
	err = qm.Publish(queueName, []byte(message))
	if err != nil {
		t.Fatalf("Failed to publish message: %v", err)
	}

	// Wait for message to be received
	select {
	case receivedMsg := <-received:
		if receivedMsg != message {
			t.Errorf("Received message = %v, want %v", receivedMsg, message)
		}
	case <-time.After(2 * time.Second):
		t.Error("Timeout waiting for message")
	}
}

func TestQueueManager_DeclareQueue(t *testing.T) {
	if testing.Short() {
		t.Skip("skipping integration test")
	}

	qm := NewQueueManager()
	err := qm.Connect("amqp://guest:guest@localhost:5672/")
	if err != nil {
		t.Fatalf("Failed to connect to RabbitMQ: %v", err)
	}
	defer qm.Close()

	queueName := "test_queue"
	err = qm.DeclareQueue(queueName)
	if err != nil {
		t.Errorf("Failed to declare queue: %v", err)
	}
}
