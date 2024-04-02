package app

import (
	"context"
	"fmt"
	"github.com/robfig/cron"
	"log"
	"net/http"
	"time"
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
	removeExpiredIdempotencyHistory(walletService)

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

// removeExpiredIdempotencyHistory call RemoveExpire method every hour
func removeExpiredIdempotencyHistory(service services.WalletService) {
	c := cron.New()
	err := c.AddFunc("@every 5s", func() {
		if err := service.RemoveExpire(time.Now().Add(-1 * time.Hour)); err != nil {
			log.Println(err.Error())
		}
	})

	if err != nil {
		log.Println(err.Error())
	}

	c.Start()
}
