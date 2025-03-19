package application

import (
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestNew(t *testing.T) {
	app := New()
	if app == nil {
		t.Error("New() returned nil")
	}
}

func TestApplication_Run(t *testing.T) {
	tests := []struct {
		name    string
		port    int
		wantErr bool
	}{
		{
			name:    "valid port",
			port:    8080,
			wantErr: false,
		},
		{
			name:    "invalid port",
			port:    -1,
			wantErr: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			app := New()
			err := app.Run(tt.port)
			if (err != nil) != tt.wantErr {
				t.Errorf("Application.Run() error = %v, wantErr %v", err, tt.wantErr)
			}
		})
	}
}

func TestApplication_HealthCheck(t *testing.T) {
	app := New()
	server := httptest.NewServer(http.HandlerFunc(app.HealthCheck))
	defer server.Close()

	resp, err := http.Get(server.URL + "/health")
	if err != nil {
		t.Fatalf("Failed to make request: %v", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		t.Errorf("HealthCheck() status = %v, want %v", resp.StatusCode, http.StatusOK)
	}
}

func TestApplication_Shutdown(t *testing.T) {
	app := New()
	err := app.Shutdown()
	if err != nil {
		t.Errorf("Application.Shutdown() error = %v", err)
	}
} 