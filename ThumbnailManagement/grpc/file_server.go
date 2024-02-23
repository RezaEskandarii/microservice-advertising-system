package grpc

import (
	"context"
	"database/sql"
	"fmt"
	"github.com/google/uuid"
	"google.golang.org/grpc"
	"log"
	"net"
	"path/filepath"
	"slices"
	"strings"
	pb "thumbnail-management/grpc/thumbnail-management/grpc"
	"thumbnail-management/internal/database"
	"thumbnail-management/internal/models"
	"thumbnail-management/internal/repositories"
	"thumbnail-management/internal/services"
	"thumbnail-management/pkg/env_manager"
)

var (
	thumbnailService       *services.ThumbnailService
	db                     *sql.DB
	objectStorageAddr      = env_manager.Load("object_storage_address")
	objectStorageAccessKey = env_manager.Load("object_storage_access_key")
	objectStorageSecretKey = env_manager.Load("object_storage_secret_key")
)

// init initializes the necessary dependencies for the package.
func init() {
	// Initialize the storage manager
	sm, err := services.NewStorageManager(objectStorageAddr, objectStorageAccessKey, objectStorageSecretKey)
	if err != nil {
		// panic(err.Error())
	}

	// Initialize the database
	db = database.GetDb(context.Background())

	// Create a PostgreSQL repository for thumbnails
	thumbnailRepo := repositories.NewPostgreSQLRepository(db)
	// Create a ThumbnailService with the repository and storage manager
	thumbnailService = services.NewThumbnailService(thumbnailRepo, sm)
}

// FileServer represents a gRPC file server.
type FileServer struct {
	pb.FileServiceServer
}

// NewFileServer creates a new instance of FileServer.
func NewFileServer() FileServer {
	return FileServer{}
}

// UploadFile handles the upload of a file.
func (s *FileServer) UploadFile(ctx context.Context, req *pb.FileRequest) (*pb.FileResponse, error) {
	// List of allowed file extensions
	allowedExtensions := []string{".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp"}

	// Extract the extension of the file
	ext := strings.ToLower(filepath.Ext(req.FileName))
	// Check if the file extension is allowed
	if !slices.Contains(allowedExtensions, ext) {
		return nil, fmt.Errorf("invalid file extension: %s", req.GetFileName())
	}

	// Generate a unique filename
	uuidStr := uuid.New()
	fileName := fmt.Sprintf("%s_%s", uuidStr.String(), req.GetFileName())

	// Prepare the response
	response := &pb.FileResponse{
		Success:  true,
		Message:  "File uploaded successfully",
		FileName: fileName,
	}

	// Create a Thumbnail model
	thumbnail := models.Thumbnail{
		ImageName:       fileName,
		ImageBucket:     "",
		ImageBytes:      req.GetFileContent(),
		AdvertisementID: req.GetAdvertisementId(),
	}

	// Create the thumbnail using the ThumbnailService
	thumbnailService.CreateThumbnail(&thumbnail)
	return response, nil
}

// Start starts the gRPC server on the specified port.
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
