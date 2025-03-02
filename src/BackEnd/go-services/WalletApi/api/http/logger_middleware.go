package http

import (
	"github.com/RezaEskandarii/ad-go-commons/logger"
	"net/http"
	"time"

	"github.com/google/uuid"
)

// responseRecorder to capture status code
type responseRecorder struct {
	http.ResponseWriter
	statusCode int
}

func (r *responseRecorder) WriteHeader(code int) {
	r.statusCode = code
	r.ResponseWriter.WriteHeader(code)
}

// requestLoggerMiddleware middleware
func requestLoggerMiddleware(next http.HandlerFunc, appLogger logger.AppLogger) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		start := time.Now()
		traceID := uuid.New().String()

		// Create a response recorder
		rec := &responseRecorder{ResponseWriter: w, statusCode: http.StatusOK}

		next.ServeHTTP(rec, r)

		userID, _ := GetUserId(r)

		// Log request details
		requestLog := logger.RequestLog{
			TraceID:            traceID,
			Method:             r.Method,
			Path:               r.URL.Path,
			QueryString:        r.URL.RawQuery,
			UserAgent:          r.UserAgent(),
			IpAddress:          r.RemoteAddr,
			ResponseStatusCode: rec.statusCode,
			ResponseTimeMs:     time.Since(start).Milliseconds(),
			UserId:             userID,
			Error:              "",
			Time:               time.Now(),
		}

		appLogger.Info("RequestLoggerMiddleware", &requestLog)
	}
}
