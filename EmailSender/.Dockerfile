# Use the official Golang image as the base image
FROM golang:1.21

# Set the working directory inside the container
WORKDIR /app

# Copy the Go module files
COPY go.mod go.sum ./

# Download the Go module dependencies
RUN go mod download

# Copy the source code into the container
COPY . .

# Build the Go application
RUN go build -o app

# Set the environment variables
ENV RABBITMQ_CONNECTION_STRING="amqp://guest:guest@rabbitmq:5672/"
ENV EMAIL_SERVICE_API_KEY="your-email-service-api-key"

# Expose the port on which the application listens
EXPOSE 8080

# Set the command to run the application when the container starts
CMD ["./app"]