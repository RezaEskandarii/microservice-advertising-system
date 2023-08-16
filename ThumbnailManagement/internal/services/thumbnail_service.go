package services

import (
	. "thumbnail-management/internal/models"
	. "thumbnail-management/internal/repositories"
)

type ThumbnailService struct {
	thumbnailRepository ThumbnailRepository
}

func NewThumbnailService(thumbnailRepository ThumbnailRepository) *ThumbnailService {
	return &ThumbnailService{
		thumbnailRepository: thumbnailRepository,
	}
}

func (s *ThumbnailService) DeleteThumbnail(id int) error {
	return s.thumbnailRepository.DeleteThumbnail(id)
}

func (s *ThumbnailService) GetThumbnailByID(id int) (*Thumbnail, error) {
	return s.thumbnailRepository.FindByID(id)
}

func (s *ThumbnailService) CreateThumbnail(thumbnail *Thumbnail) (*Thumbnail, error) {
	return s.thumbnailRepository.Create(thumbnail)
}

func (s *ThumbnailService) UpdateThumbnail(id int, thumbnail *Thumbnail) (*Thumbnail, error) {
	return s.thumbnailRepository.Update(id, thumbnail)
}

func (s *ThumbnailService) GetThumbnailsByCategory(categoryID int) ([]Thumbnail, error) {
	return s.thumbnailRepository.FindAll(categoryID)
}
