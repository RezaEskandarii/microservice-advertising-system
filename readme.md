# Online Advertisement Website - Docker Compose Setup

This repository contains the Docker Compose setup for an online advertisement website built with microservices architecture using C# (with CQRS pattern) and Golang. The project utilizes various technologies and services such as PostgreSQL, RabbitMQ, Vault, Consul, Minio, Elasticsearch, Kibana, Redis, and multiple microservices written in C#(ASP.NET Core) and Golang.

## Prerequisites
- Docker
- Docker Compose

## Services

### PostgreSQL
- Image: `postgres:latest`
- Default database: `postgres`
- Username: `postgres`
- Password: `123456`

### RabbitMQ
- Image: `rabbitmq:management`
- Ports: `5672` (AMQP) and `15672` (Management UI)
- Default username: `guest`
- Default password: `guest`

### Vault
- Image: `hashicorp/vault:latest`
- Development root token: `58ec2667-a9f4-455b-b89b-914fe565b7ea`

### Consul
- Image: `hashicorp/consul:latest`
- Development mode enabled
- Accessible at `http://localhost:8500`

### Minio
- Image: `minio/minio`
- Accessible at `http://localhost:9000`
- Username: `minioadmin`
- Password: `minioadmin`

### Elasticsearch
- Image: `docker.elastic.co/elasticsearch/elasticsearch:7.15.2`
- Accessible at `http://localhost:9200` (HTTP) and `9300` (TCP transport)

### Kibana
- Image: `docker.elastic.co/kibana/kibana:7.4.0`
- Accessible at `http://localhost:5601`
- Dependency: Elasticsearch

### Redis
- Image: `redis:latest`

### Identity Management API
- Dockerfile: `./IdentityManagement/Dockerfile`
- Port: `5004`
- Dependencies: PostgreSQL, Vault, Consul

### Advertisement Management API
- Dockerfile: `./AdvertisementManagement/Dockerfile`
- Port: `5006`
- Dependencies: PostgreSQL, Vault, Consul, Elasticsearch

### Email Sender API
- Dockerfile: `./EmailSender/Dockerfile`
- Port: `5007`
- Dependencies: Vault, Consul, RabbitMQ

### Thumbnail Management API
- Dockerfile: `./ThumbnailManagement/Dockerfile`
- Port: `5002`
- Dependencies: Minio, PostgreSQL, Vault, Consul

### Category Management API
- Dockerfile: `./CategoryManagement/Dockerfile`
- Dependencies: PostgreSQL, Vault, Consul

### Location Management API
- Dockerfile: `./LocationManagement/Dockerfile`
- Port: `5010`
- Dependencies: PostgreSQL, Vault

### API Gateway Application API
- Dockerfile: `./ApiGateway/Dockerfile`
- Port: `5009`
- Dependencies: Vault, PostgreSQL, Identity Management API, Category Management API, Location Management API, Advertisement Management API

## Usage
1. Clone this repository.
2. Navigate to the project directory in your terminal.
3. Run `docker-compose up -d` to start all services in detached mode.
4. Access the project via `http://localhost:5009` once the services are up and running.

## Contributing
Contributions are welcome! If you find any issues or want to contribute enhancements, feel free to open a pull request.

## License
This project is licensed under the [MIT License](LICENSE).
