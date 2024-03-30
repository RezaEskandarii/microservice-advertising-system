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

func (s *TransactionServer) Register(service services.WalletService) {
	s.service = service

	var port = fmt.Sprintf(":%s", env_manager.LoadEnv("wallet_grpc_port"))
	lis, err := net.Listen("tcp", port)
	if err != nil {
		log.Fatalf("failed to listen: %v", err)
	}

	gs := grpc.NewServer()
	pb.RegisterTransactionServiceServer(gs, &TransactionServer{service: service})
	fmt.Printf("###### transaction grpc server listen on: %s ######", port)

	if err := gs.Serve(lis); err != nil {
		log.Fatalf("failed to serve: %v", err)
	}
}

func (s *TransactionServer) ProcessTransaction(ctx context.Context, in *pb.Transaction) (*pb.TransactionResult, error) {
	var err = s.service.Withdrawal(in.UserId, in.Amount, in.IdempotencyKey)
	if err != nil {
		return &pb.TransactionResult{Status: false, ErrorMessage: err.Error()}, nil
	}

	return &pb.TransactionResult{Status: true}, nil
}
