package http

import (
	"encoding/json"
	"errors"
	"fmt"
	"github.com/golang-jwt/jwt/v4"
	"net/http"
	"strings"
	"wallet-api/pkg/env_manager"
)

var secretKey = []byte(env_manager.LoadEnv("jwt_secret"))

// GetUserId extracts the user ID from the JWT token in the Authorization header
func GetUserId(r *http.Request) (string, error) {
	authHeader := r.Header.Get("Authorization")
	if authHeader == "" {
		return "", errors.New("HttpRequest header does not contain Bearer token")
	}

	// Extract the token from the Authorization header
	tokenString := strings.TrimPrefix(authHeader, "Bearer ")

	// Parse the token
	token, err := jwt.Parse(tokenString, func(token *jwt.Token) (interface{}, error) {
		if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
			return nil, fmt.Errorf("unexpected signing method: %v", token.Header["alg"])
		}
		return secretKey, nil
	})

	if err != nil {
		//	return "", fmt.Errorf("could not process JWT token: %v", err)
	}

	if claims, ok := token.Claims.(jwt.MapClaims); ok { //} && token.Valid {
		userId, ok := claims["id"].(string)
		if !ok {
			return "", errors.New("id claim not found in token")
		}
		return userId, nil
	}

	return "", errors.New("invalid token")
}

func setJsonContentType(w http.ResponseWriter) {
	w.Header().Set("Content-Type", "application/json")
}

func writeData(data interface{}, w http.ResponseWriter) {
	jsonData, err := json.MarshalIndent(data, "", " ")
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	w.Write(jsonData)
}
