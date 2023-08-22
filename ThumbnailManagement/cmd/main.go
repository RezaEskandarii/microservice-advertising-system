package main

import (
	"fmt"
	"github.com/hashicorp/consul/api"
	"github.com/hashicorp/consul/connect"
	"github.com/hashicorp/vault/api"
	_ "github.com/lib/pq"
	"log"
	"os"
	"os/signal"
	"syscall"
)

func main() {
	app := NewApp()
	app.Run(5002)
}

// RegisterOnServiceRegistry registers service address on service discovery address
func RegisterOnServiceRegistry() {
	// Create a new Consul client
	client, err := api.NewClient(api.DefaultConfig())
	if err != nil {
		log.Fatal(err)
	}

	// Create a new service registration
	reg := &api.AgentServiceRegistration{
		ID:   "my-service-1",
		Name: "My Service",
		Tags: []string{"tag1", "tag2"},
		Port: 8080,
		Check: &api.AgentServiceCheck{
			TCP:      "localhost:8080",
			Interval: "10s",
			Timeout:  "2s",
		},
	}

	// Register the service with Consul
	err = client.Agent().ServiceRegister(reg)
	if err != nil {
		log.Fatal(err)
	}

	fmt.Println("Service registered with Consul")

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
