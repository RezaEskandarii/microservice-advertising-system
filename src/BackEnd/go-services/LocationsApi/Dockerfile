FROM golang:1.21.0

# Set the working directory inside the container
WORKDIR /LocationServiceApp

# Copy the Go module files to the working directory
COPY go.mod go.sum ./

# Download and install the Go dependencies
RUN go mod download

# Copy the source code to the working directory
COPY . .

COPY locations.json ./

RUN go build ./cmd/main.go 

EXPOSE 5010

# Set the entry point of the container
CMD ["./main"]