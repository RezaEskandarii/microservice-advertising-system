package grpc

import (
	"context"
	"database/sql"
	"fmt"
	"github.com/google/uuid"
	"google.golang.org/grpc"
	"log"
	"net"
	pb "thumbnail-management/grpc/thumbnail-management/grpc"
	"thumbnail-management/internal/database"
	"thumbnail-management/internal/models"
	"thumbnail-management/internal/repositories"
	"thumbnail-management/internal/services"
)

var (
	thumbnailService *services.ThumbnailService
	db               *sql.DB
)

func init() {
	sm, err := services.NewStorageManager("127.0.0.1:9000", "minioadmin", "minioadmin")
	if err != nil {
		//	panic(err.Error())
	}

	ctx := context.Background()
	db = database.GetDb(ctx)

	thumbnailRepo := repositories.NewPostgreSQLRepository(db)
	thumbnailService = services.NewThumbnailService(thumbnailRepo, sm)
}

type FileServer struct {
	pb.FileServiceServer
}

func NewFleServer() FileServer {
	return FileServer{}
}

func (s *FileServer) UploadFile(ctx context.Context, req *pb.FileRequest) (*pb.FileResponse, error) {

	// Return a success response
	response := &pb.FileResponse{
		Success: true,
		Message: "File uploaded successfully",
	}
	uuidStr := uuid.New()
	thumbnail := models.Thumbnail{
		ImageName:       fmt.Sprintf("%s_%s", uuidStr.String(), req.GetFileName()),
		ImageBucket:     "",
		ImageBytes:      req.GetFileContent(),
		AdvertisementID: req.GetAdvertisementId(),
	}

	thumbnailService.CreateThumbnail(&thumbnail)
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
