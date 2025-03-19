package models

import (
	"errors"
	"fmt"
	"net/mail"
)

type Advertisement struct {
	Title     string `json:"title"`
	UserEmail string `json:"user_email"`
}

func (ad Advertisement) Validate() error {
	if ad.Title == "" {
		return errors.New("title is required")
	}
	if ad.UserEmail == "" {
		return errors.New("user email is required")
	}
	if _, err := mail.ParseAddress(ad.UserEmail); err != nil {
		return fmt.Errorf("invalid email format: %v", err)
	}
	return nil
}

func (ad Advertisement) String() string {
	return fmt.Sprintf("Advertisement{Title: %s, UserEmail: %s}", ad.Title, ad.UserEmail)
}
