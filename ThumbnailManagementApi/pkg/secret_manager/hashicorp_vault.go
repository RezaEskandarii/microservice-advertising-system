package secret_manager

var (
	mountPath  = "secret"
	secretPath = "thumbnail-microservice"
)

//
//type SecretManager struct {
//	client *vault.Client
//}
//
//func New() *SecretManager {
//
//	config := vault.DefaultConfig()
//	config.Address = env.LoadEnv("vault_address")
//	client, err := vault.NewClient(config)
//
//	if err != nil {
//		///	panic(err.Error())
//	}
//
//	client.SetToken(env.LoadEnv("vault_token"))
//	return &SecretManager{
//		client: client,
//	}
//}
//
//func (s *SecretManager) Put(ctx context.Context, key, val string) error {
//
//	secretData := make(map[string]interface{})
//	secretData[key] = val
//
//	_, err := s.client.KVv2(mountPath).Put(ctx, secretPath, secretData)
//	return err
//}
//
//func (s *SecretManager) Get(ctx context.Context, key string) (interface{}, error) {
//
//	secret, err := s.client.KVv2(mountPath).Get(ctx, secretPath)
//	if err != nil {
//		return nil, err
//	}
//
//	value, ok := secret.Data[key].(string)
//	if !ok {
//		return nil, fmt.Errorf(
//			"value type assertion failed: %T %#v",
//			secret.Data[key],
//			secret.Data[key],
//		)
//	}
//
//	return value, nil
//
//}
//
//func (s *SecretManager) GetConnectionString(ctx context.Context, dbName string) string {
//	// TODO: read from hashicorp vault
//
//	db := fmt.Sprintf(" dbname=%s ", dbName)
//	if strings.TrimSpace(dbName) == "" {
//		db = ""
//	}
//	return fmt.Sprintf("user=postgres password=boofhichkas %s host=127.0.0.1 port=5432 sslmode=disable", db)
//}
