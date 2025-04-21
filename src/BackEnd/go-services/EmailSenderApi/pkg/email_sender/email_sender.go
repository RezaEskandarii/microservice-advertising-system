package email_sender

import (
	"errors"
	"fmt"
	"net/mail"
	"strings"
)

type SendEmailRequest struct {
	Subject    string  `json:"title"`
	Body       string  `json:"body"`
	To         string  `json:"to"`
	Attachment *[]byte `json:"attachment"`
}

func (r SendEmailRequest) Validate() error {
	var errorMessages []string

	if r.To == "" {
		errorMessages = append(errorMessages, "recipient email is required")
	} else if !isValidEmail(r.To) {
		errorMessages = append(errorMessages, fmt.Sprintf("invalid email format: %v", err))
	}

	if r.Subject == "" {
		errorMessages = append(errorMessages, "subject is required")
	}

	if r.Body == "" {
		errorMessages = append(errorMessages, "body is required")
	}

	if len(errorMessages) > 0 {
		return errors.New("Email validation errors:\n - " + strings.Join(errorMessages, "\n - "))
	}

	return nil
}

func isValidEmail(email string) bool {
	_, err := mail.ParseAddress(email)
	return err == nil
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
