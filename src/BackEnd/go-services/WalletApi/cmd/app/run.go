package app

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"wallet-api/api/grpc"
	api "wallet-api/api/http"
	"wallet-api/internal/database"
	"wallet-api/internal/repositories"
	"wallet-api/internal/services"
	"wallet-api/pkg/env_manager"
)

func Run(ctx context.Context) error {
	// Initialize the database
	if err := database.Init(ctx); err != nil {
		return err
	}

	// Get the wallet API database connection
	db, err := database.GetWalletApiDB(ctx)
	if err != nil {
		return err
	}
	defer db.Close() // Close the database connection when the function returns

	// Create a new wallet repository and service
	walletRepository := repositories.NewPostgreSQLRepository(db)
	walletService := services.NewWalletService(walletRepository)

	// Create a new wallet handler and initialize the routes
	handler := api.NewWalletHandler(walletService)
	handler.InitRoutes()

	// Remove expired idempotency history
	go func() {
		if err := walletService.RemoveExpiredIdempotencies(); err != nil {
			log.Fatal(err.Error())
		}
	}()

	// Register and start the transaction gRPC server
	gs := grpc.TransactionServer{}
	go gs.Register(walletService)

	// Load the wallet API port from the environment
	port := env_manager.LoadEnv("wallet_api_port")
	fmt.Printf("######## wallet api started on: %s #######\n", port)

	// Start the HTTP server
	if err := http.ListenAndServe(fmt.Sprintf(":%s", port), nil); err != nil {
		return err
	}

	return nil
}
