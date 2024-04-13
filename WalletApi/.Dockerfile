FROM golang:latest

WORKDIR /EmailSenderApp

COPY go.mod go.sum ./

RUN go mod download

COPY . .

RUN go build ./cmd/main.go

EXPOSE 5005
EXPOSE 6000

CMD ["./main"]