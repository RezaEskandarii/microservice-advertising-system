# Microservice Advertising System

A modern, scalable online advertising platform built with microservices architecture, combining the power of .NET Core and GoLang services. This system provides a complete solution for managing advertisements, user identities, categories, locations, and more.

## 🚀 Features

- **Microservices Architecture**: Distributed system design for scalability and maintainability
- **Multi-language Support**: Services built with both .NET Core and GoLang
- **CQRS Pattern**: Command Query Responsibility Segregation for optimized data operations
- **Service Discovery**: Using Consul for dynamic service registration and discovery
- **Secret Management**: Secure handling of sensitive data with HashiCorp Vault
- **Message Queue**: Asynchronous communication using RabbitMQ
- **Search Capabilities**: Full-text search powered by Elasticsearch
- **Monitoring**: Comprehensive monitoring with Prometheus and Grafana
- **Object Storage**: File storage using MinIO
- **Caching**: Redis for high-performance caching
- **API Gateway**: Centralized request routing and management

## 🏗️ Architecture

The system consists of several microservices:

1. **Identity Management API** (.NET Core)
   - User authentication and authorization
   - JWT token management
   - User profile management

2. **Advertisement Management API** (.NET Core)
   - Core advertisement functionality
   - Ad creation, modification, and deletion
   - Search and filtering capabilities

3. **Email Sender API** (GoLang)
   - Asynchronous email processing
   - Email template management
   - Notification system

4. **Thumbnail Management API** (GoLang)
   - Object storage integration
   - Thumbnail generation

5. **Category Management API** (GoLang)
   - Category hierarchy management
   - Caching layer for performance
   - Category-based filtering

6. **Location Management API** (GoLang)
   - Geographic data management
   - Location-based services

7. **Wallet API** (GoLang)
   - Payment processing
   - Transaction management
   - Balance tracking

## 🛠️ Prerequisites

- Docker
- Docker Compose
- Git

## 🚀 Getting Started

### Docker Setup

1. **Clone the Repository**
   ```bash
   git clone https://github.com/RezaEskandarii/microservice-advertising-system.git
   cd microservice-advertising-system
   ```

2. **Environment Setup**
   - Navigate to the deployment directory:
     ```bash
     cd deployment
     ```
   - The `.env` file contains all necessary environment variables. Make sure it's properly configured.
   - Default credentials and ports are pre-configured in the `.env` file:
     - PostgreSQL: port 5432
     - RabbitMQ: ports 5672 (AMQP) and 15672 (Management UI)
     - Consul: port 8500
     - MinIO: port 9000
     - Elasticsearch: ports 9200 and 9300
     - Kibana: port 5601
     - Redis: port 6379
     - API Gateway: port 5009

3. **Start the Services**
   ```bash
   # Start all services in detached mode
   docker-compose up -d

   # To view logs of all services
   docker-compose logs -f

   # To view logs of a specific service
   docker-compose logs -f [service-name]
   ```

4. **Verify Services**
   ```bash
   # Check running containers
   docker-compose ps

   # Check service health
   curl http://localhost:8500/v1/health/service/[service-name]
   ```

5. **Access the Services**
   - API Gateway: `http://localhost:5009`
   - RabbitMQ Management: `http://localhost:15672` (default credentials: guest/guest)
   - Consul UI: `http://localhost:8500`
   - Kibana: `http://localhost:5601`
   - Grafana: `http://localhost:3000` (default credentials: admin/admin)
   - MinIO: `http://localhost:9000` (default credentials: minioadmin/minioadmin)
   - Elasticsearch: `http://localhost:9200`

6. **Stop Services**
   ```bash
   # Stop all services
   docker-compose down

   # Stop and remove volumes
   docker-compose down -v
   ```

7. **Troubleshooting**
   - If services fail to start, check logs:
     ```bash
     docker-compose logs [service-name]
     ```
   - To restart a specific service:
     ```bash
     docker-compose restart [service-name]
     ```
   - To rebuild and restart a service:
     ```bash
     docker-compose up -d --build [service-name]
     ```

## 📊 Monitoring

The system includes comprehensive monitoring capabilities:

- **Prometheus**: Metrics collection
- **Grafana**: Visualization and dashboards
- **Elasticsearch & Kibana**: Log aggregation and analysis

## 🔒 Security

- JWT-based authentication
- HashiCorp Vault for secret management
- Secure communication between services
- Role-based access control

## 🧪 Testing

The project includes:
- Load testing scripts in the `load-tests` directory
- Postman collection for API testing
- Integration tests for each service

## 🤝 Contributing

Contributions are welcome! Please follow these steps:
1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For support, please open an issue in the repository or contact the maintainers.
