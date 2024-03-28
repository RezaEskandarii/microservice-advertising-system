package secret_manager

import (
	env "category-management/pkg/env_manager"
	"context"
	"fmt"
	vault "github.com/hashicorp/vault/api"
)

var (
	mountPath  = "secret"
	secretPath = "microservice-project"
)

// SecretManager is a struct that manages secrets using Vault
type SecretManager struct {
	client *vault.Client
}

// New creates a new instance of SecretManager
func New() *SecretManager {
	// Initialize Vault configuration
	config := vault.DefaultConfig()
	config.Address = env.Load("vault_address")

	// Create a new Vault client
	client, err := vault.NewClient(config)
	if err != nil {
		// Handle error if client creation fails
		// panic(err.Error())
	}

	// Set the client token for authentication
	client.SetToken(env.Load("vault_token"))

	return &SecretManager{
		client: client,
	}
}

// Put stores a key-value pair in the secret store
func (s *SecretManager) Put(ctx context.Context, key, val string) error {
	// Create a map to hold the secret data
	secretData := make(map[string]interface{})
	secretData[key] = val

	// Put the secret data in the specified path in Vault
	_, err := s.client.KVv2(mountPath).Put(ctx, secretPath, secretData)
	return err
}

// Get retrieves the value associated with a key from the secret store
func (s *SecretManager) Get(ctx context.Context, key string) (interface{}, error) {
	// Get the secret data from the specified path in Vault
	secret, err := s.client.KVv2(mountPath).Get(ctx, secretPath)
	if err != nil {
		return nil, err
	}

	// Check if the value associated with the key is a string and return it
	value, ok := secret.Data[key].(string)
	if !ok {
		return nil, fmt.Errorf(
			"value type assertion failed: %T %#v",
			secret.Data[key],
			secret.Data[key],
		)
	}

	return value, nil
}
