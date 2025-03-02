package main

import (
	"github.com/RezaEskandarii/ad-go-commons/env_manager"
	logger2 "github.com/RezaEskandarii/ad-go-commons/logger"
	_ "github.com/lib/pq"
	"log"
	"strconv"
	"thumbnail-management/cmd/application"
)

func main() {

	app := application.New()
	logger, err := logger2.NewElasticLogger(env_manager.GetString("elasticsearch_url"), "thumbnail-app")

	if err != nil {
		log.Fatal(err.Error())
	}

	portNumberStr := env_manager.GetString("thumbnail_grpc_api_port_number")
	portNumber, err := strconv.ParseInt(portNumberStr, 10, 64)

	if err != nil {
		logger.Error(err.Error(), nil)
	}

	if err := app.Run(portNumber); err != nil {
		logger.Error(err.Error(), nil)
	}
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
