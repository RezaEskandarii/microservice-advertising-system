package email_sender

import "fmt"

type SendEmailRequest struct {
	Subject    string  `json:"title"`
	Body       string  `json:"body"`
	Attachment *[]byte `json:"attachment"`
}

type EmailSender interface {
	Send(request SendEmailRequest) error
}

type EmailSenderImpl struct {
	EmailSender
}

func (receiver SendEmailRequest) Send(request SendEmailRequest) error {
	fmt.Println(receiver)
	return nil
}
