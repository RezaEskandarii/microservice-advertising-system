package env_manager

import (
	"os"
	"testing"
)

func TestLoad(t *testing.T) {
	tests := []struct {
		name     string
		key      string
		value    string
		want     string
		wantErr  bool
		setEnv   bool
	}{
		{
			name:    "existing environment variable",
			key:     "TEST_ENV_VAR",
			value:   "test_value",
			want:    "test_value",
			wantErr: false,
			setEnv:  true,
		},
		{
			name:    "non-existing environment variable",
			key:     "NON_EXISTING_VAR",
			want:    "",
			wantErr: true,
			setEnv:  false,
		},
		{
			name:    "empty environment variable",
			key:     "EMPTY_VAR",
			value:   "",
			want:    "",
			wantErr: true,
			setEnv:  true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if tt.setEnv {
				os.Setenv(tt.key, tt.value)
				defer os.Unsetenv(tt.key)
			}

			got, err := Load(tt.key)
			if (err != nil) != tt.wantErr {
				t.Errorf("Load() error = %v, wantErr %v", err, tt.wantErr)
			}
			if got != tt.want {
				t.Errorf("Load() = %v, want %v", got, tt.want)
			}
		})
	}
}

func TestLoadWithDefault(t *testing.T) {
	tests := []struct {
		name     string
		key      string
		value    string
		defaultValue string
		want     string
		setEnv   bool
	}{
		{
			name:    "existing environment variable",
			key:     "TEST_ENV_VAR",
			value:   "test_value",
			defaultValue: "default_value",
			want:    "test_value",
			setEnv:  true,
		},
		{
			name:    "non-existing environment variable",
			key:     "NON_EXISTING_VAR",
			defaultValue: "default_value",
			want:    "default_value",
			setEnv:  false,
		},
		{
			name:    "empty environment variable",
			key:     "EMPTY_VAR",
			value:   "",
			defaultValue: "default_value",
			want:    "default_value",
			setEnv:  true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if tt.setEnv {
				os.Setenv(tt.key, tt.value)
				defer os.Unsetenv(tt.key)
			}

			got := LoadWithDefault(tt.key, tt.defaultValue)
			if got != tt.want {
				t.Errorf("LoadWithDefault() = %v, want %v", got, tt.want)
			}
		})
	}
} 