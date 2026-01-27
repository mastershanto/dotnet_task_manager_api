# 🏢 Enterprise-Grade Task Manager API

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)

> **World-class enterprise REST API for task management built with Clean Architecture, CQRS, and DDD patterns.**

## 🌟 Key Features

### Architecture Excellence
- ✅ **Clean Architecture** - Domain, Application, Infrastructure, and Presentation layers
- ✅ **CQRS Pattern** - Command Query Responsibility Segregation with MediatR
- ✅ **Domain-Driven Design** - Rich domain entities, value objects, and domain events
- ✅ **Repository Pattern** - Generic repositories with Unit of Work
- ✅ **Result Pattern** - Type-safe error handling without exceptions

### Enterprise Security
- 🔐 **JWT Authentication** - Industry-standard token-based auth with refresh tokens
- 🔐 **Role-Based Authorization** - Fine-grained access control
- 🔐 **Security Headers** - HSTS, CSP, X-Frame-Options, and more (OWASP compliant)
- 🔐 **Rate Limiting** - Multi-strategy protection against abuse
- 🔐 **Secret Management** - Environment-based configuration with Azure Key Vault support

### Performance & Scalability
- ⚡ **Redis Caching** - Distributed caching for high performance
- ⚡ **Response Compression** - Brotli and Gzip compression
- ⚡ **Connection Pooling** - Optimized database connections
- ⚡ **Async/Await** - Non-blocking I/O operations throughout

### Observability
- 📊 **Structured Logging** - Serilog with file and console sinks
- 📊 **OpenTelemetry** - Distributed tracing and metrics
- 📊 **Health Checks** - Database, Redis, and application health monitoring
- 📊 **API Versioning** - Backward-compatible API evolution

### Developer Experience
- 📚 **Swagger/OpenAPI** - Interactive API documentation
- 📚 **FluentValidation** - Declarative input validation
- 📚 **MediatR Pipeline** - Cross-cutting concerns (logging, validation, performance)
- 📚 **Docker Ready** - Multi-stage Dockerfile and Docker Compose

---

## 🚀 Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Docker](https://www.docker.com/get-started) (optional, for containerized deployment)
- [PostgreSQL 14+](https://www.postgresql.org/download/) (or use Docker Compose)
- [Redis 6+](https://redis.io/download/) (or use Docker Compose)

### Option 1: Docker Compose (Recommended)

```bash
# Clone the repository
git clone <repository-url>
cd dotnet_task_manager_api

# Copy environment file
cp .env.example .env
# Edit .env and set your secrets (JWT_SECRET_KEY, passwords, etc.)

# Start all services (API, PostgreSQL, Redis)
docker-compose up -d

# View logs
docker-compose logs -f api

# Access the API
# Swagger UI: http://localhost:5000/swagger
# API Base:   http://localhost:5000/api
```

### Option 2: Local Development

```bash
# 1. Install dependencies
dotnet restore

# 2. Configure settings
cp .env.example .env
# Edit .env with your configuration

# 3. Setup database (PostgreSQL)
# Update ConnectionStrings in appsettings.Development.json
dotnet ef database update

# 4. Run the application
dotnet run

# Access the API
# HTTPS: https://localhost:5001
# HTTP:  http://localhost:5000
# Swagger: https://localhost:5001/swagger
```

---

## 📋 API Documentation

### Authentication Endpoints
```http
POST   /api/auth/register          # Register new user
POST   /api/auth/login             # Login and get JWT token
POST   /api/auth/refresh           # Refresh access token
POST   /api/auth/logout            # Logout user
```

### Task Management Endpoints
```http
GET    /api/v1/tasks               # Get all tasks (paginated)
GET    /api/v1/tasks/{id}          # Get task by ID
POST   /api/v1/tasks               # Create new task
PUT    /api/v1/tasks/{id}          # Update task
DELETE /api/v1/tasks/{id}          # Delete task
PATCH  /api/v1/tasks/{id}/status   # Update task status
PATCH  /api/v1/tasks/{id}/assign   # Assign task to user
```

### Health Check Endpoints
```http
GET    /health                     # Detailed health check (DB, Redis)
GET    /health/live                # Liveness probe (Kubernetes)
```

Full API documentation available at `/swagger` when running in development mode.

---

## 🏗️ Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────┐
│       Presentation Layer                │
│  Controllers, DTOs, API Responses       │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│       Application Layer                 │
│  CQRS Handlers, Validators, Behaviors   │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│        Domain Layer                     │
│  Entities, Value Objects, Events        │
└─────────────────────────────────────────┘
                 ▲
┌────────────────┴────────────────────────┐
│     Infrastructure Layer                │
│  Repositories, DbContext, Services      │
└─────────────────────────────────────────┘
```

### Project Structure
```
├── Application/              # Business logic & CQRS
│   ├── Abstractions/        # Interfaces
│   ├── Features/            # Feature-based organization
│   │   └── Tasks/           # Task feature
│   │       ├── Commands/    # Write operations
│   │       ├── Queries/     # Read operations
│   │       └── DomainEventHandlers/
│   └── Common/
│       └── Behaviors/       # MediatR pipeline behaviors
├── Domain/                  # Core business entities
│   ├── Entities/            # Domain entities
│   ├── ValueObjects/        # Value objects
│   ├── Events/              # Domain events
│   └── Exceptions/          # Domain exceptions
├── Infrastructure/          # External concerns
│   ├── Persistence/         # EF Core, repositories
│   └── Repositories/        # Repository implementations
├── Presentation/            # API contracts
│   └── DTOs/                # Data transfer objects
├── Controllers/             # API controllers
├── Middleware/              # Custom middleware
├── Services/                # Application services
└── Validation/              # FluentValidation rules
```

---

## 🔐 Security Features

### JWT Token Service
- ✅ Strongly-typed configuration with `IOptions<JwtSettings>`
- ✅ Configuration validation at startup (fail-fast)
- ✅ Minimum 256-bit symmetric keys (HMAC-SHA256)
- ✅ Standard JWT claims (jti, iat, nbf, sub)
- ✅ UTC timestamps for timezone safety
- ✅ Structured logging for audit trails
- ✅ Refresh token support (7-day default expiry)

### Security Headers (OWASP Compliant)
```
✓ Strict-Transport-Security (HSTS)
✓ Content-Security-Policy (CSP)
✓ X-Content-Type-Options
✓ X-Frame-Options
✓ X-XSS-Protection
✓ Referrer-Policy
✓ Permissions-Policy
```

### Rate Limiting Strategies
- **Fixed Window**: 100 req/min per IP (general endpoints)
- **Sliding Window**: 200 req/min (authenticated users)
- **Token Bucket**: Burst protection with average rate
- **Concurrency Limiter**: Max 20 concurrent requests per user
- **Auth Endpoints**: 5 req/min (brute-force protection)
- **Per-User Limits**: 300 req/min sliding window
- **Per-IP Limits**: 50 req/min for public endpoints

---

## 📊 Monitoring & Observability

### Structured Logging
```csharp
// Serilog with enrichment
Log.Information("JWT token created for user {UserId}", user.Id);
Log.Warning("Authentication failed for {Path}", context.Request.Path);
```

### Health Checks
```bash
# Detailed health check (includes dependencies)
curl http://localhost:5000/health

# Liveness probe (Kubernetes)
curl http://localhost:5000/health/live
```

### OpenTelemetry Integration
- Distributed tracing with OTLP exporter
- Runtime metrics collection
- HTTP instrumentation
- Custom spans for business operations

---

## 🐳 Docker Deployment

### Build Docker Image
```bash
docker build -t todo-api:latest .
```

### Run with Docker Compose
```bash
# Production deployment
docker-compose up -d

# Development with PgAdmin
docker-compose --profile dev up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Environment Variables
See [`.env.example`](.env.example) for all configuration options.

**Critical Environment Variables:**
- `JWT_SECRET_KEY` - Minimum 32 characters (generate with `openssl rand -base64 48`)
- `POSTGRES_PASSWORD` - PostgreSQL password
- `REDIS_PASSWORD` - Redis password
- `DATABASE_CONNECTION_STRING` - Full PostgreSQL connection string

---

## 🧪 Testing

```bash
# Run unit tests
dotnet test tests/TodoApi.UnitTests

# Run integration tests
dotnet test tests/TodoApi.IntegrationTests

# Run all tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## 📦 Dependencies

### Core Framework
- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core** - ORM
- **Npgsql** - PostgreSQL provider

### CQRS & Validation
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation

### Authentication & Security
- **JWT Bearer Authentication** - Token-based auth
- **ASP.NET Core Rate Limiting** - Built-in rate limiting

### Caching & Performance
- **StackExchange.Redis** - Redis client
- **Response Compression** - Brotli/Gzip

### Logging & Observability
- **Serilog** - Structured logging
- **OpenTelemetry** - Tracing and metrics

### Documentation
- **Swashbuckle (Swagger)** - API documentation

---

## 🚦 Performance Optimization

### Response Compression
- Brotli compression (optimal level)
- Gzip fallback
- HTTPS-enabled compression

### Database Optimization
- Connection pooling
- Retry on failure (max 5 retries)
- Indexed queries
- Async operations

### Caching Strategy
- Redis distributed cache
- LRU eviction policy
- 256MB memory limit

---

## 🌍 Production Deployment

### Azure Deployment
```bash
# Azure Container Apps
az containerapp up --name todo-api \
  --resource-group todo-rg \
  --environment todo-env \
  --image todo-api:latest

# Configure secrets in Azure Key Vault
az keyvault secret set --vault-name todo-kv \
  --name JwtSecretKey --value "your-secret-key"
```

### Kubernetes Deployment
```yaml
# See ENTERPRISE_DEPLOYMENT_GUIDE.md for full K8s manifests
apiVersion: apps/v1
kind: Deployment
metadata:
  name: todo-api
spec:
  replicas: 3
  # ... (full configuration in deployment guide)
```

### Environment-Specific Configuration
- **Development**: `appsettings.Development.json`
- **Staging**: `appsettings.Staging.json` + environment variables
- **Production**: `appsettings.Production.json` + Azure Key Vault

---

## 📖 Additional Documentation

- [**Enterprise Deployment Guide**](ENTERPRISE_DEPLOYMENT_GUIDE.md) - Complete deployment instructions
- [**Architecture Refactoring**](ARCHITECTURE_REFACTORING.md) - Clean architecture details
- [**Enterprise API Documentation**](ENTERPRISE_API.md) - Full API specification
- [**Environment Variables**](.env.example) - Configuration template

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🏆 Best Practices Implemented

✅ **Clean Architecture** - Separation of concerns  
✅ **SOLID Principles** - Maintainable and extensible code  
✅ **DRY Principle** - Avoid code duplication  
✅ **KISS Principle** - Keep it simple and straightforward  
✅ **YAGNI Principle** - You aren't gonna need it  
✅ **Fail-Fast** - Configuration validation at startup  
✅ **12-Factor App** - Cloud-native best practices  
✅ **Security First** - OWASP security guidelines  
✅ **Observable** - Comprehensive logging and monitoring  
✅ **Testable** - Unit and integration test support  

---

## 📞 Support

For issues, questions, or feature requests, please open an issue on GitHub.

**Built with ❤️ using Clean Architecture, CQRS, and Enterprise Patterns**
