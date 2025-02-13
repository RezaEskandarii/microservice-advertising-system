package env_manager

import (
	"github.com/joho/godotenv"
	"os"
)

func LoadEnv(key string) string {
	godotenv.Load()
	return os.Getenv(key)
}

func Set(key string, value string) error {
	return os.Setenv(key, value)
}

func Load(key string) string {
	return os.Getenv(key)
}