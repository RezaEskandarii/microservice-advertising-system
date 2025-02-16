package env_manager

import (
	"github.com/joho/godotenv"
	"os"
)

func Load(key string) string {
	err := godotenv.Load()
	if err != nil {
		return ""
	}
	return os.Getenv(key)
}

func GetFromOsENV(key string) string {
	return os.Getenv(key)
}
