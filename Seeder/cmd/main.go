package main

import (
	"context"
	"encoding/json"
	_ "github.com/lib/pq"
	"log"
	"os"
	"thumbnail-management/pkg/secret_manager"
)

func main() {

	defer func() {
		if r := recover(); r != nil {
			printError(r)
			return
		}
	}()

	log.Println("============= start to seeding secrets ===========")

	s := secret_manager.New()
	ctx := context.Background()

	fileData, err := os.ReadFile("./data.json")
	if err != nil {
		printError(err.Error())
		return
	}

	// Create a map to store the JSON data
	var secretData map[string]interface{}

	// Unmarshal JSON data into the map
	err = json.Unmarshal(fileData, &secretData)
	if err != nil {
		printError(err.Error())
		return
	}

	err = s.Put(ctx, secretData, 10)

	if err != nil && err.Error() != "" {
		printError(err.Error())
		return
	}

	log.Println("################ Secret seeding was done successfully!!! #######################")
}

func printError(err interface{}) {
	log.Println("#################################################################")
	log.Printf("########################## %s ########################", err)
	log.Println("#################################################################")
}
