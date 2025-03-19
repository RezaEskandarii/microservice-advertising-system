package models

import (
	"encoding/json"
	"testing"
)

func TestAdvertisement_Validate(t *testing.T) {
	tests := []struct {
		name    string
		ad      Advertisement
		wantErr bool
	}{
		{
			name: "valid advertisement",
			ad: Advertisement{
				Title:     "Test Ad",
				UserEmail: "test@example.com",
			},
			wantErr: false,
		},
		{
			name: "empty title",
			ad: Advertisement{
				Title:     "",
				UserEmail: "test@example.com",
			},
			wantErr: true,
		},
		{
			name: "empty email",
			ad: Advertisement{
				Title:     "Test Ad",
				UserEmail: "",
			},
			wantErr: true,
		},
		{
			name: "invalid email format",
			ad: Advertisement{
				Title:     "Test Ad",
				UserEmail: "invalid-email",
			},
			wantErr: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			err := tt.ad.Validate()
			if (err != nil) != tt.wantErr {
				t.Errorf("Advertisement.Validate() error = %v, wantErr %v", err, tt.wantErr)
			}
		})
	}
}

func TestAdvertisement_JSONMarshaling(t *testing.T) {
	tests := []struct {
		name     string
		ad       Advertisement
		wantJSON string
	}{
		{
			name: "valid advertisement",
			ad: Advertisement{
				Title:     "Test Ad",
				UserEmail: "test@example.com",
			},
			wantJSON: `{"title":"Test Ad","user_email":"test@example.com"}`,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// Test marshaling
			got, err := json.Marshal(tt.ad)
			if err != nil {
				t.Errorf("json.Marshal() error = %v", err)
				return
			}
			if string(got) != tt.wantJSON {
				t.Errorf("json.Marshal() = %v, want %v", string(got), tt.wantJSON)
			}

			// Test unmarshaling
			var unmarshaled Advertisement
			err = json.Unmarshal(got, &unmarshaled)
			if err != nil {
				t.Errorf("json.Unmarshal() error = %v", err)
				return
			}
			if unmarshaled != tt.ad {
				t.Errorf("json.Unmarshal() = %v, want %v", unmarshaled, tt.ad)
			}
		})
	}
}

func TestAdvertisement_String(t *testing.T) {
	tests := []struct {
		name     string
		ad       Advertisement
		wantStr  string
	}{
		{
			name: "valid advertisement",
			ad: Advertisement{
				Title:     "Test Ad",
				UserEmail: "test@example.com",
			},
			wantStr: "Advertisement{Title: Test Ad, UserEmail: test@example.com}",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := tt.ad.String(); got != tt.wantStr {
				t.Errorf("Advertisement.String() = %v, want %v", got, tt.wantStr)
			}
		})
	}
} 