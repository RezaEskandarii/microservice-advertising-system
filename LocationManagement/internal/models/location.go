package models

type City struct {
	Name      string  `json:"name"`
	Latitude  float64 `json:"latitude"`
	Longitude float64 `json:"longitude"`
}

type Country struct {
	Name   string `json:"name"`
	Cities []City `json:"cities"`
}

type Location struct {
	Countries []Country `json:"countries"`
}

type GetCountry struct {
	CountryName string `json:"country_name"`
}

type GetCity struct {
	LocationId int    `json:"location_id"`
	CityName   string `json:"city_name"`
	Pos        string `json:"position"`
}
