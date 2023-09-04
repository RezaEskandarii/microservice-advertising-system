# Use the official Go image as the base image
FROM golang:latest

# Set the working directory inside the container
WORKDIR /seederapp

# Copy the Go module files to the working directory
COPY go.mod go.sum ./

# Download and install the Go dependencies
RUN go mod download

# Copy the source code to the working directory
COPY . .
COPY data.json /seederapp/data.json

# Build the Go application
RUN go build -o cmmd/main .

# Expose port 5002
EXPOSE 5002

# Set the entry point of the container
CMD ["./main"]