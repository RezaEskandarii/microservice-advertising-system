package app

import (
	"context"
	"fmt"
	"net/http"
	"wallet-api/api/grpc"
	api "wallet-api/api/http"
	"wallet-api/internal/database"
	"wallet-api/internal/repositories"
	"wallet-api/internal/services"
	"wallet-api/pkg/env_manager"
)

func Run(ctx context.Context) error {
	if err := database.Init(ctx); err != nil {
		return err
	}

	db, err := database.GetWalletApiDB(ctx)
	if err != nil {
		return err
	}
	defer db.Close()

	walletRepository := repositories.NewPostgreSQLRepository(db)
	walletService := services.NewWalletService(walletRepository)

	handler := api.NewWalletHandler(walletService)
	handler.InitRoutes()

	gs := grpc.TransactionServer{}
	go gs.Register(walletService)

	port := env_manager.LoadEnv("wallet_api_port")
	fmt.Printf("######## wallet api started on: %s #######\n", port)

	if err := http.ListenAndServe(fmt.Sprintf(":%s", port), nil); err != nil {
		return err
	}

	return nil
}
