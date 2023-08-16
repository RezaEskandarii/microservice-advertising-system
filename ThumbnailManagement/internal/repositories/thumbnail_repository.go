package repositories

import (
	"database/sql"
	"fmt"
	_ "github.com/lib/pq"
	. "thumbnail-management/internal/models"
)

type ThumbnailRepository interface {
	DeleteThumbnail(id int) error
	FindByID(id int) (*Thumbnail, error)
	Create(Thumbnail *Thumbnail) (*Thumbnail, error)
	Update(id int, Thumbnail *Thumbnail) (*Thumbnail, error)
	FindAll(advertisementID int) ([]Thumbnail, error)
}

type PostgreSQLThumbnailRepository struct {
	db *sql.DB
}

func NewPostgreSQLRepository(db *sql.DB) *PostgreSQLThumbnailRepository {
	return &PostgreSQLThumbnailRepository{
		db: db,
	}
}

func (r *PostgreSQLThumbnailRepository) DeleteThumbnail(id int) error {
	query := "DELETE FROM thumbnails WHERE id =  $1"
	_, err := r.db.Exec(query, id)
	if err != nil {
		return err
	}
	return nil
}

func (r *PostgreSQLThumbnailRepository) FindByID(id int) (*Thumbnail, error) {
	query := "SELECT id, image_name, image_bucket, image_bytes, advertisement_id FROM thumbnails WHERE id =  $1"
	row := r.db.QueryRow(query, id)

	thumbnail := &Thumbnail{}
	err := row.Scan(&thumbnail.ID, &thumbnail.ImageName, &thumbnail.ImageBucket, &thumbnail.ImageBytes, &thumbnail.AdvertisementID)
	if err != nil {
		if err == sql.ErrNoRows {
			return nil, fmt.Errorf("Thumbnail not found")
		}
		return nil, err
	}

	return thumbnail, nil
}

func (r *PostgreSQLThumbnailRepository) Create(thumbnail *Thumbnail) (*Thumbnail, error) {
	query := "INSERT INTO thumbnails (image_name, image_bucket, advertisement_id) VALUES ( $1,  $2,  $3) RETURNING id"
	err := r.db.QueryRow(query, thumbnail.ImageName, thumbnail.ImageBucket, thumbnail.AdvertisementID).Scan(&thumbnail.ID)
	if err != nil {
		return nil, err
	}

	return thumbnail, nil
}

func (r *PostgreSQLThumbnailRepository) Update(id int, thumbnail *Thumbnail) (*Thumbnail, error) {
	query := "UPDATE thumbnails SET image_name =  $1, image_bucket =  $2, image_bytes =  $3, advertisement_id =  $4 WHERE id =  $5"
	_, err := r.db.Exec(query, thumbnail.ImageName, thumbnail.ImageBucket, thumbnail.ImageBytes, thumbnail.AdvertisementID, id)
	if err != nil {
		return nil, err
	}

	thumbnail.ID = id
	return thumbnail, nil
}

func (r *PostgreSQLThumbnailRepository) FindAll(advertisementID int) ([]Thumbnail, error) {
	query := "SELECT id, image_name, image_bucket, image_bytes, advertisement_id FROM thumbnails WHERE advertisement_id =  $1"
	rows, err := r.db.Query(query, advertisementID)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	thumbnails := []Thumbnail{}
	for rows.Next() {
		thumbnail := Thumbnail{}
		err := rows.Scan(&thumbnail.ID, &thumbnail.ImageName, &thumbnail.ImageBucket, &thumbnail.ImageBytes, &thumbnail.AdvertisementID)
		if err != nil {
			return nil, err
		}
		thumbnails = append(thumbnails, thumbnail)
	}

	return thumbnails, nil
}
