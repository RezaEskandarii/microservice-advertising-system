package services

import (
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

func (s *ThumbnailService) DeleteThumbnail(id int) error {
	th, _ := s.thumbnailRepository.FindByID(id)
	err := s.thumbnailRepository.DeleteThumbnail(id)

	if err == nil {
		s.storageManager.RemoveFile(BucketName, th.ImageName)
	}
	return err
}

func (s *ThumbnailService) GetThumbnailByID(id int) (*Thumbnail, error) {
	return s.thumbnailRepository.FindByID(id)
}

func (s *ThumbnailService) CreateThumbnail(thumbnail *Thumbnail) (*Thumbnail, error) {
	result, err := s.thumbnailRepository.Create(thumbnail)
	if err == nil {
		s.storageManager.UploadFile(BucketName, thumbnail.ImageName, thumbnail.ImageBytes)
	}
	return result, err
}

func (s *ThumbnailService) UpdateThumbnail(id int, thumbnail *Thumbnail) (*Thumbnail, error) {
	return s.thumbnailRepository.Update(id, thumbnail)
}

func (s *ThumbnailService) GetThumbnailsByAdvertisementID(advertisementID int) ([]Thumbnail, error) {
	return s.thumbnailRepository.FindAll(advertisementID)
}
