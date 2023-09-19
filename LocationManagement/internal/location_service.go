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
	GetAll() (map[string][]models.GetCity, error)
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

func (s *LocationServiceImp) CreateTables() error {
	sdn := env_manager.GetFromDotENV("location_management_full_sdn")

	db, err := sql.Open("postgres", sdn)
	if err != nil {
		log.Fatal(err)
		return err
	}

	err = createCountriesTable(db)
	if err != nil {
		log.Fatal(err)
		return err
	}

	err = createLocationsTable(db)
	if err != nil {
		log.Fatal(err)
		return err
	}

	return nil
}

func createCountriesTable(db *sql.DB) error {
	createCountriesTableQuery := `
		CREATE TABLE IF NOT EXISTS countries(
			id SERIAL PRIMARY KEY,
			name VARCHAR(255) NOT NULL
		);
	`

	_, err := db.Exec(createCountriesTableQuery)
	if err != nil {
		log.Println(err.Error())
		return err
	}

	return nil
}

func createLocationsTable(db *sql.DB) error {
	createLocationsTableQuery := `
		CREATE TABLE IF NOT EXISTS locations(
			id SERIAL PRIMARY KEY,
			country_id INT NOT NULL,
			name VARCHAR(255) NOT NULL,
			pos POINT
		);
	`

	_, err := db.Exec(createLocationsTableQuery)
	if err != nil {
		return err
	}

	return nil
}

func (s *LocationServiceImp) Seed() error {
	file, err := readLocationsJson()

	data, err := ioutil.ReadAll(file)
	if err != nil {
		log.Println("Error reading file:", err)
		return err
	}

	var location models.Location
	err = json.Unmarshal(data, &location)
	if err != nil {
		log.Println("Error:", err)
		return err
	}

	db := s.db
	for _, country := range location.Countries {
		countryID, err := getCountryID(db, country.Name)
		if err != nil {
			log.Fatal(err)
		}

		// if country is not exists by given name
		if countryID == 0 {
			countryID, err = insertCountry(db, country.Name)
			if err != nil {
				log.Fatal(err)
			}
		}

		s.insertCityFromCountry(country, db, countryID)
	}

	return nil
}

func (s *LocationServiceImp) insertCityFromCountry(country models.Country, db *sql.DB, countryID int) {
	for _, city := range country.Cities {
		cityID, err := getCityID(db, city.Name, countryID)
		if err != nil {
			log.Fatal(err)
		}

		// if city is not exists by given name
		if cityID == 0 {
			cityID, err = insertCity(db, city.Name, countryID, city.Latitude, city.Longitude)
			if err != nil {
				log.Fatal(err)
			}
			fmt.Printf("City ID for %s: %d\n", city.Name, cityID)
		}
	}
}

func readLocationsJson() (*os.File, error) {
	file, err := os.Open("locations.json")
	if err != nil {
		log.Println("Error opening file:", err)
		return nil, err
	}
	defer file.Close()

	return file, nil
}

func getCountryID(db *sql.DB, countryName string) (int, error) {
	var countryID int
	err := db.QueryRow("SELECT id FROM countries WHERE name = $1", countryName).Scan(&countryID)
	if err != nil && err != sql.ErrNoRows {
		return 0, err
	}
	return countryID, nil
}

func insertCountry(db *sql.DB, countryName string) (int, error) {
	var countryID int
	err := db.QueryRow("INSERT INTO countries(name) VALUES($1) RETURNING id", countryName).Scan(&countryID)
	if err != nil {
		return 0, err
	}
	return countryID, nil
}

func getCityID(db *sql.DB, cityName string, countryID int) (int, error) {
	var cityID int
	err := db.QueryRow("SELECT id FROM locations WHERE name = $1 AND country_id = $2", cityName, countryID).Scan(&cityID)
	if err != nil && err != sql.ErrNoRows {
		return 0, err
	}
	return cityID, nil
}

func insertCity(db *sql.DB, cityName string, countryID int, latitude float64, longitude float64) (int, error) {
	var cityID int
	err := db.QueryRow("INSERT INTO locations (name, country_id, pos) VALUES($1, $2, POINT($3, $4) ) RETURNING id",
		cityName, countryID, latitude, longitude).Scan(&cityID)
	if err != nil {
		return 0, err
	}
	return cityID, nil
}

func (s *LocationServiceImp) GetAll() (map[string][]models.GetCity, error) {

	db := s.db
	data := make(map[string][]models.GetCity)

	rows, err := db.Query(`SELECT c."name" as CountryName, l."id" as LocationId, l."name" as CityName, l."pos" as pos FROM "countries" c join "locations" l on c."id" = l."country_id"`)

	if err != nil {
		log.Fatal(err)
	}

	defer rows.Close()

	for rows.Next() {
		var country models.GetCountry
		var city models.GetCity

		err := rows.Scan(&country.CountryName, &city.LocationId, &city.CityName, &city.Pos)
		if err != nil && err != sql.ErrNoRows {
			log.Fatal(err)
			return nil, err
		}

		data[country.CountryName] = append(data[country.CountryName], city)
	}

	if err := rows.Err(); err != nil && rows.Err() != sql.ErrNoRows {
		log.Fatal(err)
	}

	return data, nil
}
