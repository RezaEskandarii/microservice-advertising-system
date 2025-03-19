package email_sender

import (
	"testing"
)

func TestEmailSender_Send(t *testing.T) {
	tests := []struct {
		name    string
		request SendEmailRequest
		wantErr bool
	}{
		{
			name: "valid email request",
			request: SendEmailRequest{
				Subject: "Test Subject",
				Body:    "Test Body",
				To:      "test@example.com",
			},
			wantErr: false,
		},
		{
			name: "empty recipient",
			request: SendEmailRequest{
				Subject: "Test Subject",
				Body:    "Test Body",
				To:      "",
			},
			wantErr: true,
		},
		{
			name: "empty subject",
			request: SendEmailRequest{
				Subject: "",
				Body:    "Test Body",
				To:      "test@example.com",
			},
			wantErr: true,
		},
		{
			name: "empty body",
			request: SendEmailRequest{
				Subject: "Test Subject",
				Body:    "",
				To:      "test@example.com",
			},
			wantErr: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			sender := EmailSenderImpl{}
			err := sender.Send(tt.request)
			if (err != nil) != tt.wantErr {
				t.Errorf("EmailSender.Send() error = %v, wantErr %v", err, tt.wantErr)
			}
		})
	}
}

func TestSendEmailRequest_Validate(t *testing.T) {
	tests := []struct {
		name    string
		request SendEmailRequest
		wantErr bool
	}{
		{
			name: "valid request",
			request: SendEmailRequest{
				Subject: "Test Subject",
				Body:    "Test Body",
				To:      "test@example.com",
			},
			wantErr: false,
		},
		{
			name: "invalid email format",
			request: SendEmailRequest{
				Subject: "Test Subject",
				Body:    "Test Body",
				To:      "invalid-email",
			},
			wantErr: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			err := tt.request.Validate()
			if (err != nil) != tt.wantErr {
				t.Errorf("SendEmailRequest.Validate() error = %v, wantErr %v", err, tt.wantErr)
			}
		})
	}
} 