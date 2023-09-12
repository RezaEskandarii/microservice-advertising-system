package internal

import (
	"database/sql"
	"encoding/json"
	"fmt"
	_ "github.com/lib/pq"
	"io/ioutil"
	"location-management/internal/models"
	"location-management/pkg/env_manager"
	"log"
	"os"
)

type LocationService interface {
	Seed() error
	GetAll() (map[models.GetCountry][]models.GetCity, error)
}

type LocationServiceImp struct {
	db *sql.DB
	LocationService
}

const (
	dbName = "location_management"
)

func NewLocationService(db *sql.DB) *LocationServiceImp {
	return &LocationServiceImp{db: db}
}

func (s *LocationServiceImp) CreateDB() error {

	sdn := env_manager.GetFromDotENV("location_management_base_sdn")

	// Connect to db
	db, err := sql.Open("postgres", sdn)
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	// Check if the database already exists
	var exists bool
	err = db.QueryRow("SELECT EXISTS (SELECT 1 FROM pg_catalog.pg_database WHERE datname = $1)", dbName).Scan(&exists)
	if err != nil {
		return err
	}

	// If the database does not exist, create it
	if !exists {
		_, err = db.Exec("CREATE DATABASE " + dbName)
		if err != nil {
			return err
		}
	}

	return nil
}

func (s *LocationServiceImp) CreateTable() (int64, error) {

	sdn := env_manager.GetFromDotENV("location_management_full_sdn")

	// Connect to db
	db, err := sql.Open("postgres", sdn)
	if err != nil {
		log.Fatal(err)
	}

	createCountriesTableQuery := `
		CREATE TABLE IF NOT EXISTS countries(
			id SERIAL PRIMARY KEY,
			name VARCHAR(255) NOT NULL
		);

	`

	r, err := db.Exec(createCountriesTableQuery)
	if err != nil {
		log.Println(r)
		return 0, err
	}

	createLocationsTableQuery := `
		CREATE TABLE IF NOT EXISTS locations(
			id SERIAL PRIMARY KEY,
			country_id INT NOT NULL,
			name VARCHAR(255) NOT NULL,
			pos POINT
		);

	`
	result, err := db.Exec(createLocationsTableQuery)
	if err != nil {
		return 0, err
	}

	return result.RowsAffected()
}

func (s *LocationServiceImp) Seed() error {

	file, err := os.Open("locations.json")
	if err != nil {
		log.Println("Error opening file:", err)
		return err
	}
	defer file.Close()
	db := s.db

	// Read the JSON data from the file
	data, err := ioutil.ReadAll(file)
	if err != nil {
		log.Println("Error reading file:", err)
		return err
	}

	// Create a location struct to hold the data
	var location models.Location

	// Unmarshal the JSON data into the struct
	err = json.Unmarshal([]byte(data), &location)
	if err != nil {
		log.Println("Error:", err)
		return err
	}

	// Access and print the data
	for _, country := range location.Countries {

		// Check if the country already exists
		var countryID int
		err = db.QueryRow("SELECT id FROM countries WHERE name = $1", country.Name).Scan(&countryID)
		if err != nil && err != sql.ErrNoRows {
			log.Fatal(err)
		}

		if countryID == 0 {
			// Insert the country if it doesn't exist and return its ID
			err = db.QueryRow("INSERT INTO countries(name) VALUES($1) RETURNING id", country.Name).Scan(&countryID)
			if err != nil {
				log.Fatal(err)
			}
		}

		fmt.Printf("Country ID: %d\n", countryID)

		// Insert cities associated with the country, checking for duplicates
		for _, city := range country.Cities {
			// Check if the city already exists for the country
			var cityID int
			err := db.QueryRow("SELECT id FROM locations WHERE name = $1 AND country_id = $2", city.Name, countryID).Scan(&cityID)
			if err != nil && err != sql.ErrNoRows {
				log.Fatal(err)
			}

			if cityID == 0 {
				// Insert the city if it doesn't exist and return its ID
				err = db.QueryRow("INSERT INTO locations (name, country_id,pos) VALUES($1, $2, POINT($3, $4) ) RETURNING id",
					city.Name, countryID, city.Latitude, city.Longitude).Scan(&cityID)
				if err != nil {
					log.Fatal(err)
				}
			}

			fmt.Printf("City ID for %s: %d\n", city.Name, cityID)

		}
	}
	return nil
}

func (s *LocationServiceImp) GetAll() (map[models.GetCountry][]models.GetCity, error) {

	db := s.db
	data := make(map[models.GetCountry][]models.GetCity)

	rows, err := db.Query(`SELECT c."name" as CountryName, l."id" as LocationId, l."name" as CityName FROM "countries" c join "locations" l on c."id" = l."country_id"`)

	if err != nil {
		log.Fatal(err)
	}

	defer rows.Close()

	for rows.Next() {
		var country models.GetCountry
		var city models.GetCity

		err := rows.Scan(&country.CountryName, &city.LocationId, &city.CityName)
		if err != nil {
			log.Fatal(err)
			return nil, err
		}

		data[country] = append(data[country], city)
	}

	if err := rows.Err(); err != nil {
		log.Fatal(err)
	}

	return data, nil
}
