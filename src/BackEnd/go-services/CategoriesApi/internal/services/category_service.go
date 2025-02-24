package services

import (
	. "category-management/internal/models"
	. "category-management/internal/repositories"
	"context"
	"encoding/json"
	"fmt"
	"github.com/redis/go-redis/v9"
	"time"
)

type CategoryService struct {
	repo  CategoryRepository
	redis *redis.Client
}

func NewCategoryService(repo CategoryRepository, redisClient *redis.Client) *CategoryService {
	return &CategoryService{
		repo:  repo,
		redis: redisClient,
	}
}

func (s *CategoryService) FindByID(id int) (*Category, error) {
	ctx := context.Background()
	cacheKey := fmt.Sprintf("category:%d", id)

	// Check if the category exists in Redis
	cachedData, err := s.redis.Get(ctx, cacheKey).Result()
	if err == nil {
		var category Category
		if err := json.Unmarshal([]byte(cachedData), &category); err == nil {
			return &category, nil // Return cached category
		}
	}

	// Fetch from repository if not in cache
	category, err := s.repo.FindByID(id)
	if err != nil {
		return nil, err
	}

	// Store in Redis with a 10-minute expiration time
	data, _ := json.Marshal(category)
	s.redis.Set(ctx, cacheKey, data, 24*time.Minute)

	return category, nil
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

func (s *CategoryService) FindAll() ([]Category, error) {
	ctx := context.Background()
	cacheKey := "categories:all"

	// Check if data exists in Redis
	cachedData, err := s.redis.Get(ctx, cacheKey).Result()
	if err == nil {
		var categories []Category
		if err := json.Unmarshal([]byte(cachedData), &categories); err == nil {
			return categories, nil // Return cached data
		}
	}

	// Fetch from repository if not in cache
	categories, err := s.repo.FindAll()
	if err != nil {
		return nil, err
	}

	// Store in Redis with a 10-minute expiration time
	data, _ := json.Marshal(categories)
	s.redis.Set(ctx, cacheKey, data, 24*time.Hour)

	return categories, nil
}

func (s *CategoryService) Seed() error {
	return s.repo.Seed()
}
