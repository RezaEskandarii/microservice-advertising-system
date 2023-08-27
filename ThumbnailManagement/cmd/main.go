package main

import (
	"fmt"
	"github.com/hashicorp/consul/api"
	_ "github.com/lib/pq"
	"log"
	"os"
	"os/signal"
	"syscall"
)

func main() {
	app := NewApp()

	portNumber := 5002
	go registerOnServiceRegistry(portNumber)

	app.Run(portNumber)
}

// RegisterOnServiceRegistry registers service address on service discovery address
func registerOnServiceRegistry(port int) {
	// Create a new Consul client
	client, err := api.NewClient(api.DefaultConfig())
	if err != nil {
		log.Fatal(err)
	}

	// Create a new service registration
	reg := &api.AgentServiceRegistration{
		ID:      "thumbnail-service",
		Name:    "thumbnails management service",
		Tags:    []string{"thumbnails management", "upload thumbnails"},
		Port:    port,
		Address: "http://127.0.0.1",
		Check: &api.AgentServiceCheck{
			TCP:      fmt.Sprintf("localhost:%d", port),
			Interval: "10s",
			Timeout:  "2s",
		},
	}

	// Register the service with Consul
	err = client.Agent().ServiceRegister(reg)
	if err != nil {
		log.Fatal(err)
	}

	fmt.Printf("Service registered with Consul on port: %d \n", port)

	// Handle termination signals to deregister the service gracefully
	sigCh := make(chan os.Signal, 1)
	signal.Notify(sigCh, syscall.SIGINT, syscall.SIGTERM)

	<-sigCh

	// Deregister the service from Consul
	err = client.Agent().ServiceDeregister(reg.ID)
	if err != nil {
		log.Fatal(err)
	}

	fmt.Println("Service deregistered from Consul")
}
