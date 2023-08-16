package services

import (
	"bytes"
	"context"
	"github.com/minio/minio-go/v7"
	"log"
	"net/url"

	"github.com/minio/minio-go/v7/pkg/credentials"
)

type StorageManager interface {
	EnsureBucketExists(bucketName string) error
	UploadFile(bucketName string, objectName string, fileBytes []byte) error
	DownloadFile(bucketName string, objectName string, filePath string) error
	GetDirectLink(bucketName string, objectName string) (string, error)
}

type StorageManagerImpl struct {
	minioClient *minio.Client
}

func NewStorageManager(endpoint string, accessKey string, secretKey string) (StorageManager, error) {
	minioClient, err := minio.New(endpoint, &minio.Options{
		Creds:  credentials.NewStaticV4(accessKey, secretKey, ""),
		Secure: true,
	})
	if err != nil {
		return nil, err
	}

	return &StorageManagerImpl{
		minioClient: minioClient,
	}, nil
}

func (s *StorageManagerImpl) EnsureBucketExists(bucketName string) error {
	ctx := context.Background()

	exists, err := s.minioClient.BucketExists(ctx, bucketName)
	if err != nil {
		return err
	}

	if !exists {
		err = s.minioClient.MakeBucket(ctx, bucketName, minio.MakeBucketOptions{})
		if err != nil {
			return err
		}
		log.Println("Bucket created successfully.")
	} else {
		log.Println("Bucket already exists.")
	}

	return nil
}

func (s *StorageManagerImpl) UploadFile(bucketName string, objectName string, fileBytes []byte) error {
	ctx := context.Background()

	_, err := s.minioClient.PutObject(ctx, bucketName, objectName, bytes.NewReader(fileBytes), int64(len(fileBytes)), minio.PutObjectOptions{})
	if err != nil {
		return err
	}

	log.Println("File uploaded successfully.")
	return nil
}

func (s *StorageManagerImpl) DownloadFile(bucketName string, objectName string, filePath string) error {
	ctx := context.Background()

	err := s.minioClient.FGetObject(ctx, bucketName, objectName, filePath, minio.GetObjectOptions{})
	if err != nil {
		return err
	}

	log.Println("File downloaded successfully.")
	return nil
}

func (s *StorageManagerImpl) GetDirectLink(bucketName string, objectName string) (string, error) {
	ctx := context.Background()

	reqParams := url.Values{}
	presignedURL, err := s.minioClient.PresignedGetObject(ctx, bucketName, objectName, 24*60*60, reqParams)
	if err != nil {
		return "", err
	}

	return presignedURL.String(), nil
}
