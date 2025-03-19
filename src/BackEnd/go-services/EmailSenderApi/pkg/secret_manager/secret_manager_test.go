package secret_manager

import (
	"testing"
)

func TestSecretManager_GetSecret(t *testing.T) {
	tests := []struct {
		name     string
		path     string
		key      string
		wantErr  bool
	}{
		{
			name:    "valid secret path and key",
			path:    "secret/data/test",
			key:     "test_key",
			wantErr: false,
		},
		{
			name:    "invalid secret path",
			path:    "invalid/path",
			key:     "test_key",
			wantErr: true,
		},
		{
			name:    "empty path",
			path:    "",
			key:     "test_key",
			wantErr: true,
		},
		{
			name:    "empty key",
			path:    "secret/data/test",
			key:     "",
			wantErr: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			sm := NewSecretManager()
			_, err := sm.GetSecret(tt.path, tt.key)
			if (err != nil) != tt.wantErr {
				t.Errorf("SecretManager.GetSecret() error = %v, wantErr %v", err, tt.wantErr)
			}
		})
	}
}

func TestSecretManager_Initialize(t *testing.T) {
	tests := []struct {
		name     string
		config   VaultConfig
		wantErr  bool
	}{
		{
			name: "valid configuration",
			config: VaultConfig{
				Address: "http://localhost:8200",
				Token:   "test-token",
			},
			wantErr: false,
		},
		{
			name: "invalid address",
			config: VaultConfig{
				Address: "invalid-address",
				Token:   "test-token",
			},
			wantErr: true,
		},
		{
			name: "empty token",
			config: VaultConfig{
				Address: "http://localhost:8200",
				Token:   "",
			},
			wantErr: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			sm := NewSecretManager()
			err := sm.Initialize(tt.config)
			if (err != nil) != tt.wantErr {
				t.Errorf("SecretManager.Initialize() error = %v, wantErr %v", err, tt.wantErr)
			}
		})
	}
}

func TestSecretManager_HealthCheck(t *testing.T) {
	tests := []struct {
		name     string
		config   VaultConfig
		wantErr  bool
	}{
		{
			name: "valid configuration",
			config: VaultConfig{
				Address: "http://localhost:8200",
				Token:   "test-token",
			},
			wantErr: false,
		},
		{
			name: "invalid configuration",
			config: VaultConfig{
				Address: "invalid-address",
				Token:   "test-token",
			},
			wantErr: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			sm := NewSecretManager()
			err := sm.Initialize(tt.config)
			if err != nil {
				t.Skipf("Skipping test due to initialization error: %v", err)
			}
			err = sm.HealthCheck()
			if (err != nil) != tt.wantErr {
				t.Errorf("SecretManager.HealthCheck() error = %v, wantErr %v", err, tt.wantErr)
			}
		})
	}
} 