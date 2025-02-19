FROM golang:1.21.0 AS builder

WORKDIR /WalletApiApp

COPY go.mod go.sum ./

RUN go mod download

COPY . .

RUN go build -o main ./cmd/main.go

FROM alpine:latest

WORKDIR /root/

COPY --from=builder /WalletApiApp/main .

COPY locations.json .

RUN apk add --no-cache libc6-compat

EXPOSE 5005
EXPOSE 6000

CMD ["./main"]