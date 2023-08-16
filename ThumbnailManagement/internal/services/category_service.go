package services

import . "category-management/internal/repositories"
import . "category-management/internal/models"

type CategoryService struct {
	repo CategoryRepository
}

func NewCategoryService(repo CategoryRepository) *CategoryService {
	return &CategoryService{
		repo: repo,
	}
}

func (s *CategoryService) Create(category *Category) (*Category, error) {
	return s.repo.Create(category)
}

func (s *CategoryService) Update(id int, category *Category) (*Category, error) {
	return s.repo.Update(id, category)
}

func (s *CategoryService) DeleteCategory(id int) error {
	return s.repo.DeleteCategory(id)
}

func (s *CategoryService) FindByID(id int) (*Category, error) {
	return s.repo.FindByID(id)
}

func (s *CategoryService) FindAll() ([]Category, error) {
	return s.repo.FindAll()
}
