package app

import (
	"context"
	"fmt"
	"github.com/RezaEskandarii/ad-go-commons/env_manager"
	"github.com/RezaEskandarii/ad-go-commons/logger"
	"log"
	"net/http"
	"wallet-api/api/grpc"
	api "wallet-api/api/http"
	"wallet-api/internal/database"
	"wallet-api/internal/repositories"
	"wallet-api/internal/services"
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
	defer db.Close()

	// Create a new wallet repository and service
	walletRepository := repositories.NewPostgreSQLRepository(db)
	walletService := services.NewWalletService(walletRepository)

	// Load the wallet API port from the environment
	port := env_manager.GetString("wallet_api_port")

	elasticLogger, err := logger.NewElasticLogger(env_manager.GetString("elasticsearch_url"), "wallet-app")

	if err != nil {
		log.Fatal(err)
	}

	elasticLogger.Info(fmt.Sprintf("wallet app started on port: %s", port), nil)

	// Create a new wallet handler and initialize the routes
	handler := api.NewWalletHandler(walletService, elasticLogger)
	handler.InitRoutes()

	// Remove expired idempotency history
	go func() {
		if err := walletService.RemoveExpiredIdempotencies(); err != nil {
			elasticLogger.Error(err.Error(), nil)
		}
	}()

	// Register and start the transaction gRPC server
	gs := grpc.TransactionServer{}
	go gs.Register(walletService)

	fmt.Printf("######## wallet api started on: %s #######\n", port)

	// Start the HTTP server
	if err := http.ListenAndServe(fmt.Sprintf(":%s", port), nil); err != nil {
		return err
	}

	return nil
}
