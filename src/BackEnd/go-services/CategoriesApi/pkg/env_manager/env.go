package env_manager

import (
	"github.com/joho/godotenv"
	"os"
	"strings"
)

func Load(key string) string {
	err := godotenv.Load()
	if err != nil {
		return strings.TrimSpace(os.Getenv(key))
	}
	return strings.TrimSpace(os.Getenv(key))
}

func GetFromOsENV(key string) string {
	return os.Getenv(key)
}
