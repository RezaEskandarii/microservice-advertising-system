package services

import (
	"context"
	. "thumbnail-management/internal/models"
	. "thumbnail-management/internal/repositories"
)

const (
	BucketName = "advertising-thumbnails"
)

type ThumbnailService struct {
	thumbnailRepository ThumbnailRepository
	storageManager      StorageManager
}

func NewThumbnailService(thumbnailRepository ThumbnailRepository, storageManager StorageManager) *ThumbnailService {
	return &ThumbnailService{
		thumbnailRepository: thumbnailRepository,
		storageManager:      storageManager,
	}
}

func (s *ThumbnailService) DeleteThumbnail(ctx context.Context, id int) error {
	th, _ := s.thumbnailRepository.FindByID(id)
	err := s.thumbnailRepository.DeleteThumbnail(id)

	if err == nil {
		s.storageManager.RemoveFile(ctx, BucketName, th.ImageName)
	}
	return err
}

func (s *ThumbnailService) GetThumbnailByID(id int) (*Thumbnail, error) {
	return s.thumbnailRepository.FindByID(id)
}

func (s *ThumbnailService) CreateThumbnail(ctx context.Context, thumbnail *Thumbnail) (*Thumbnail, error) {
	thumbnail.ImageBucket = BucketName
	result, err := s.thumbnailRepository.Create(thumbnail)
	if err == nil && thumbnail.ImageBytes != nil {
		s.storageManager.UploadFile(ctx, BucketName, thumbnail.ImageName, thumbnail.ImageBytes)
	}
	return result, err
}

func (s *ThumbnailService) UpdateThumbnail(id int, thumbnail *Thumbnail) (*Thumbnail, error) {
	return s.thumbnailRepository.Update(id, thumbnail)
}

func (s *ThumbnailService) GetThumbnailsByAdvertisementID(advertisementID int) ([]Thumbnail, error) {
	return s.thumbnailRepository.FindAll(advertisementID)
}
