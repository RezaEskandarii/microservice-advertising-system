package http

import (
	"encoding/json"
	"errors"
	"fmt"
	"github.com/RezaEskandarii/ad-go-commons/env_manager"
	"github.com/RezaEskandarii/ad-go-commons/logger"
	"github.com/golang-jwt/jwt/v4"
	"github.com/google/uuid"
	"net/http"
	"strconv"
	"strings"
)

func writeJSONResponse(w http.ResponseWriter, status int, response *ApiResponse) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)

	json.NewEncoder(w).Encode(response)
}

func buildLogRequest(r *http.Request, statusCode int, error string) logger.RequestLog {
	userID, _ := GetUserId(r)
	newUUID := uuid.New()
	return logger.RequestLog{
		TraceID:            newUUID.String(),
		Method:             r.Method,
		Path:               r.URL.Path,
		QueryString:        r.URL.RawQuery,
		UserAgent:          r.UserAgent(),
		IpAddress:          r.RemoteAddr,
		ResponseStatusCode: statusCode,
		UserId:             userID,
		Error:              error,
	}
}

func logRequest(r *http.Request, err error, h *WalletHandler) {
	logRequest := buildLogRequest(r, http.StatusBadRequest, err.Error())
	h.logger.Error(err.Error(), &logRequest)
}

func parsePaginationParams(r *http.Request) (int, int) {
	page, err := strconv.Atoi(r.URL.Query().Get("page"))
	if err != nil || page < 1 {
		page = 1
	}

	perPage, err := strconv.Atoi(r.URL.Query().Get("per_page"))
	if err != nil || perPage < 1 {
		perPage = 10
	}

	return page, perPage
}

var secretKey = []byte(env_manager.GetString("jwt_secret"))

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
