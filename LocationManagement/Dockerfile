# Use the official Go image as the base image
FROM golang:latest

# Set the working directory inside the container
WORKDIR /LocationServiceApp

# Copy the Go module files to the working directory
COPY go.mod go.sum ./

# Download and install the Go dependencies
RUN go mod download

# Copy the source code to the working directory
COPY . .

# Build the Go application
RUN go build ./cmd/main.go 

# Expose port 5007
EXPOSE 5010

# Set the entry point of the container
CMD ["./main"]