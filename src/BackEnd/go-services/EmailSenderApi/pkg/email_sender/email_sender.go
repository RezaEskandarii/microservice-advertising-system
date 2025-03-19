package email_sender

import (
	"errors"
	"fmt"
	"net/mail"
)

type SendEmailRequest struct {
	Subject    string  `json:"title"`
	Body       string  `json:"body"`
	To         string  `json:"to"`
	Attachment *[]byte `json:"attachment"`
}

func (r SendEmailRequest) Validate() error {
	if r.To == "" {
		return errors.New("recipient email is required")
	}
	if _, err := mail.ParseAddress(r.To); err != nil {
		return fmt.Errorf("invalid email format: %v", err)
	}
	if r.Subject == "" {
		return errors.New("subject is required")
	}
	if r.Body == "" {
		return errors.New("body is required")
	}
	return nil
}

type EmailSender interface {
	Send(request SendEmailRequest) error
}

type EmailSenderImpl struct {
	EmailSender
}

func (receiver EmailSenderImpl) Send(request SendEmailRequest) error {
	if err := request.Validate(); err != nil {
		return err
	}
	// TODO: Implement actual email sending logic
	fmt.Printf("Sending email to %s with subject: %s\n", request.To, request.Subject)
	return nil
}
