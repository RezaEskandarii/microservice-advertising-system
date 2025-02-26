package grpc

import (
	"context"
	"fmt"
	"google.golang.org/grpc"
	"log"
	"net"
	pb "wallet-api/api/grpc/wallet-api/grpc"
	"wallet-api/internal/services"
	"wallet-api/pkg/env_manager"
)

type TransactionServer struct {
	pb.UnimplementedTransactionServiceServer
	service services.WalletService
}

// Register registers the TransactionServer with the gRPC server and starts the server
func (s *TransactionServer) Register(service services.WalletService) {
	s.service = service

	// Load the wallet gRPC port from the environment
	port := fmt.Sprintf(":%s", env_manager.LoadEnv("wallet_grpc_port"))

	// Listen for incoming gRPC connections on the specified port
	lis, err := net.Listen("tcp", port)
	if err != nil {
		log.Fatalf("failed to listen: %v", err)
	}

	// Create a new gRPC server
	gs := grpc.NewServer()

	// Register the TransactionServer with the gRPC server
	pb.RegisterTransactionServiceServer(gs, &TransactionServer{service: service})
	fmt.Printf("###### wallet grpc server listen on: %s ######", port)

	// Start the gRPC server
	if err := gs.Serve(lis); err != nil {
		log.Fatalf("failed to serve: %v", err)
	}
}

// ProcessTransaction processes a transaction request
func (s *TransactionServer) ProcessTransaction(ctx context.Context, in *pb.Transaction) (*pb.TransactionResult, error) {
	// Call the Withdrawal method on the wallet service
	err := s.service.Withdrawal(in.UserId, in.Amount, in.IdempotencyKey)
	if err != nil {
		return &pb.TransactionResult{Status: false, ErrorMessage: err.Error()}, nil
	}

	return &pb.TransactionResult{Status: true}, nil
}
