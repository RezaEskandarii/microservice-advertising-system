package models

type Thumbnail struct {
	ID              int    `json:"id"`
	ImageName       string `json:"image_name"`
	ImageBucket     string `json:"image_bucket"`
	ImageBytes      []byte `json:"image_bytes"`
	AdvertisementID int
}
