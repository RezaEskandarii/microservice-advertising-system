FROM golang:1.21.0

WORKDIR /CategoryManagementApp

# Copy the Go module files to the working directory
COPY go.mod go.sum ./

RUN go mod download

# Copy the source code to the working directory
COPY . .

COPY categories_seed_data.json ./

RUN go build ./cmd/main.go 

EXPOSE 5005

CMD ["./main"]