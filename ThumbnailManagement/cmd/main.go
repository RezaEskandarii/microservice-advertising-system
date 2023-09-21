package main

import (
	_ "github.com/lib/pq"
	"log"
	"strconv"
	"thumbnail-management/cmd/application"
	"thumbnail-management/pkg/env_manager"
)

func main() {

	app := application.New()
	portNumberStr := env_manager.LoadEnv("port_number")

	portNumber, err := strconv.ParseInt(portNumberStr, 10, 64)
	if err != nil {
		log.Fatal(err.Error())
	}
	// go registerOnServiceRegistry(portNumber)
	app.Run(portNumber)
}

// RegisterOnServiceRegistry registers service address on service discovery address
//func registerOnServiceRegistry(port int64) {
//	// Create a new Consul client
//	client, err := api.NewClient(api.DefaultConfig())
//	if err != nil {
//		log.Fatal(err)
//	}
//
//	hostName := env_manager.LoadEnv("host_name")
//
//	// Create a new service registration
//	reg := &api.AgentServiceRegistration{
//		ID:      "thumbnail-service",
//		Name:    "thumbnails management service",
//		Tags:    []string{"thumbnails management", "upload thumbnails"},
//		Port:    int(port),
//		Address: fmt.Sprintf("http://%s", hostName),
//		Check: &api.AgentServiceCheck{
//			TCP:      fmt.Sprintf("%s:%d", hostName, port),
//			Interval: "10s",
//			Timeout:  "2s",
//		},
//	}
//
//	// Register the service with Consul
//	err = client.Agent().ServiceRegister(reg)
//	if err != nil {
//		log.Fatal(err)
//	}
//
//	fmt.Printf("Service registered with Consul on port: %d \n", port)
//
//	// Handle termination signals to deregister the service gracefully
//	sigCh := make(chan os.Signal, 1)
//	signal.Notify(sigCh, syscall.SIGINT, syscall.SIGTERM)
//
//	<-sigCh
//
//	// Deregister the service from Consul
//	err = client.Agent().ServiceDeregister(reg.ID)
//	if err != nil {
//		log.Fatal(err)
//	}
//
//	fmt.Println("Service deregistered from Consul")
//}
