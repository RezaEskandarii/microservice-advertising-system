package main

import (
	"context"
	"encoding/json"
	_ "github.com/lib/pq"
	"log"
	"os"
	"thumbnail-management/pkg/secret_manager"
	"time"
)

func main() {

	defer func() {
		if r := recover(); r != nil {
			log.Fatal(r)
			return
		}
	}()

	log.Println("============= seed operation starts after 10 seconds ===========")
	delay := 10 * time.Second

	// Wait for the specified duration
	<-time.After(delay)

	s := secret_manager.New()
	ctx := context.Background()

	fileData, err := os.ReadFile("./data.json")
	if err != nil {
		log.Fatal(err.Error())
		return
	}

	// Create a map to store the JSON data
	var secretData map[string]interface{}

	// Unmarshal JSON data into the map
	err = json.Unmarshal(fileData, &secretData)
	if err != nil {
		log.Fatal(err.Error())
		return
	}

	err = s.Put(ctx, secretData, 10)

	if err != nil && err.Error() != "" {
		log.Fatal(err.Error())
	}

}
