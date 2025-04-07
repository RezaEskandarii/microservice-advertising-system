FROM golang:1.21.0 AS builder

WORKDIR /app

COPY go.mod go.sum ./
RUN go mod download

COPY . .
RUN go build -o wallet-api ./cmd/main.go

FROM alpine:latest

WORKDIR /app

USER walletuser

COPY --from=builder /app/wallet-api .

EXPOSE 5005 6000

CMD ["./wallet-api"]
