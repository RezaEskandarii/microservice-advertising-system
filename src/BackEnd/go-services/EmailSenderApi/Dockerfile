FROM golang:1.21.0

# Set the working directory inside the container
WORKDIR /EmailSenderApp

# Copy the Go module files to the working directory
COPY go.mod go.sum ./

# Download and install the Go dependencies
RUN go mod download

# Copy the source code to the working directory
COPY . .

RUN go build ./cmd/main.go 

EXPOSE 5007

CMD ["./main"]