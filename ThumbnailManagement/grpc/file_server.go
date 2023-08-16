package grpc

import (
	"context"
	"fmt"
	"google.golang.org/grpc"
	"log"
	"net"
	pb "thumbnail-management/grpc/thumbnail-management/grpc"
	"thumbnail-management/internal/models"
	"thumbnail-management/internal/services"
)

type FileServer struct {
	pb.FileServiceServer
	thumbnailService *services.ThumbnailService
}

func NewFleServer(ts *services.ThumbnailService) FileServer {
	return FileServer{
		thumbnailService: ts,
	}
}

func (s *FileServer) UploadFile(ctx context.Context, req *pb.FileRequest) (*pb.FileResponse, error) {

	// Return a success response
	response := &pb.FileResponse{
		Success: true,
		Message: "File uploaded successfully",
	}
	thumbnail := models.Thumbnail{
		ImageName:       req.GetFileName(),
		ImageBucket:     "",
		ImageBytes:      req.GetFileContent(),
		AdvertisementID: req.GetAdvertisementId(),
	}

	s.thumbnailService.CreateThumbnail(&thumbnail)
	return response, nil
}

func (s *FileServer) Start(port string) {
	// Create the gRPC server
	grpcServer := grpc.NewServer()

	// Register the file service
	fileSvc := &FileServer{}
	pb.RegisterFileServiceServer(grpcServer, fileSvc)

	// Start listening on a TCP port
	listener, err := net.Listen("tcp", fmt.Sprintf(":%s", port))
	if err != nil {
		log.Fatalf("Failed to listen: %v", err)
	}

	// Start the gRPC server
	fmt.Println("gRPC server is running on port " + port)
	if err := grpcServer.Serve(listener); err != nil {
		log.Fatalf("Failed to serve: %v", err)
	}
}
