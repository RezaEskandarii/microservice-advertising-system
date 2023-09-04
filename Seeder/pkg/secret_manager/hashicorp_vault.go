package secret_manager

import (
	"context"
	"fmt"
	vault "github.com/hashicorp/vault/api"
	"time"
)

var (
	mountPath  = "secret"
	secretPath = "microservice-project"
)

type SecretManager struct {
	client *vault.Client
}

func New() *SecretManager {

	config := vault.DefaultConfig()
	config.Address = "http://localhost:8200"
	client, err := vault.NewClient(config)

	if err != nil {
		///	panic(err.Error())
	}

	client.SetToken("58ec2667-a9f4-455b-b89b-914fe565b7ea")
	return &SecretManager{
		client: client,
	}
}

func (s *SecretManager) Put(ctx context.Context, secretData map[string]interface{}, retryCount uint32) error {

	if retryCount == 0 {
		return nil
	}
	_, err := s.client.KVv2(mountPath).Put(ctx, secretPath, secretData)
	if err != nil {
		time.Sleep(2000)
		retryCount = retryCount - 1
		s.Put(ctx, secretData, retryCount)
	}
	return err
}

func (s *SecretManager) Get(ctx context.Context, key string) (interface{}, error) {

	secret, err := s.client.KVv2(mountPath).Get(ctx, key)
	if err != nil {
		return nil, err
	}

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
