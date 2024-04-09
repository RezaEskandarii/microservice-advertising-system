package email_sender

import "fmt"

type SendEmailRequest struct {
	Subject    string  `json:"title"`
	Body       string  `json:"body"`
	To         string  `json:"to"`
	Attachment *[]byte `json:"attachment"`
}

type EmailSender interface {
	Send(request SendEmailRequest) error
}

type EmailSenderImpl struct {
	EmailSender
}

func (receiver EmailSenderImpl) Send(request SendEmailRequest) error {
	fmt.Println(request)
	return nil
}
