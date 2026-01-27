# 🏢 Enterprise-Grade Todo API - Complete Architecture Guide

## 📋 Table of Contents
- [Overview](#overview)
- [Architecture Patterns](#architecture-patterns)
- [Security Best Practices](#security-best-practices)
- [Configuration Management](#configuration-management)
- [Deployment Guide](#deployment-guide)
- [Monitoring & Observability](#monitoring--observability)
- [Performance & Scalability](#performance--scalability)

---

## 🎯 Overview

This is a **world-class enterprise-grade ASP.NET Core API** implementing:
- ✅ **Clean Architecture** (Domain, Application, Infrastructure, Presentation layers)
- ✅ **CQRS Pattern** with MediatR
- ✅ **Domain-Driven Design** (Rich Entities, Value Objects, Domain Events)
- ✅ **Repository Pattern & Unit of Work**
- ✅ **Enterprise Security** (JWT with refresh tokens, role-based authorization)
- ✅ **Structured Logging** with Serilog
- ✅ **Health Checks & Monitoring**
- ✅ **API Versioning**
- ✅ **Distributed Caching** with Redis
- ✅ **OpenTelemetry** for tracing and metrics

---

## 🏗️ Architecture Patterns

### Clean Architecture Layers

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  (Controllers, DTOs, API Responses)     │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│        Application Layer                │
│  (CQRS Handlers, Validators, Behaviors) │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│          Domain Layer                   │
│  (Entities, Value Objects, Events)      │
└─────────────────────────────────────────┘
                 ▲
┌────────────────┴────────────────────────┐
│      Infrastructure Layer               │
│  (Repositories, DbContext, Services)    │
└─────────────────────────────────────────┘
```

### Key Design Patterns

1. **CQRS (Command Query Responsibility Segregation)**
   - Commands: Modify state (CreateTaskCommand, UpdateTaskCommand)
   - Queries: Read state (GetTasksQuery, GetTaskByIdQuery)
   - MediatR pipeline behaviors for cross-cutting concerns

2. **Repository Pattern**
   - Generic repository: `IRepository<T>`
   - Specific repositories: `ITaskRepository`, `ITaskHistoryRepository`
   - Unit of Work pattern for transaction management

3. **Result Pattern**
   - No exceptions for business logic failures
   - `Result<T>` object with Success/Failure states
   - Rich error information

4. **Options Pattern**
   - Strongly-typed configuration with `IOptions<T>`
   - Configuration validation at startup (fail-fast)
   - Environment-specific settings

---

## 🔐 Security Best Practices

### JWT Token Service (Enterprise-Grade)

#### Features Implemented
```csharp
✅ Strongly-typed configuration (IOptions<JwtSettings>)
✅ Configuration validation at startup
✅ Minimum 256-bit (32-byte) symmetric keys
✅ Standard JWT claims (jti, iat, nbf, sub)
✅ UTC timestamps (timezone-safe)
✅ Structured logging for audit trails
✅ Token expiration and refresh token support
✅ Role-based claims for authorization
```

#### Token Structure
```json
{
  "sub": "user_id",           // Subject (user ID)
  "unique_name": "username",  // Username
  "email": "user@example.com",
  "jti": "unique-token-id",   // JWT ID (for revocation)
  "iat": 1705420800,          // Issued at timestamp
  "nbf": 1705420800,          // Not before timestamp
  "exp": 1705424400,          // Expiration timestamp
  "role": ["Admin", "User"]   // User roles
}
```

### Secret Management

#### Development
```bash
# appsettings.Development.json
{
  "Jwt": {
    "Key": "development-secret-key-at-least-32-characters-long"
  }
}
```

#### Production (Environment Variables)
```bash
# Use environment variables or Azure Key Vault
export JWT_SECRET_KEY=$(openssl rand -base64 48)
export DATABASE_CONNECTION_STRING="Host=prod-db;..."
export REDIS_CONNECTION_STRING="prod-redis:6379,password=..."
```

#### Azure Key Vault Integration
```csharp
// Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri("https://your-keyvault.vault.azure.net/"),
    new DefaultAzureCredential());
```

### Security Headers
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", 
        "max-age=31536000; includeSubDomains");
    await next();
});
```

---

## ⚙️ Configuration Management

### Configuration Hierarchy
```
appsettings.json                    # Base configuration
  ↓
appsettings.Development.json        # Development overrides
appsettings.Staging.json            # Staging overrides
appsettings.Production.json         # Production overrides
  ↓
Environment Variables               # Highest priority
  ↓
Azure Key Vault / AWS Secrets       # Production secrets
```

### JWT Configuration
```json
{
  "Jwt": {
    "Issuer": "TodoApi",
    "Audience": "TodoApiClients",
    "Key": "${JWT_SECRET_KEY}",
    "ExpiresMinutes": 60,
    "RefreshTokenExpiryDays": 7,
    "ClockSkewSeconds": 300
  }
}
```

### Validation at Startup
```csharp
// Fail-fast if configuration is invalid
var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>();

jwtSettings.Validate(); // Throws if invalid
```

---

## 🚀 Deployment Guide

### Prerequisites
- .NET 8.0 SDK or later
- PostgreSQL 14+
- Redis 6+
- Docker (optional)

### Local Development Setup

1. **Clone and restore dependencies**
```bash
git clone <repository>
cd dotnet_task_manager_api
dotnet restore
```

2. **Configure development settings**
```bash
cp .env.example .env
# Edit .env with your local settings
```

3. **Setup database**
```bash
# Start PostgreSQL (Docker)
docker run --name todo-postgres -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres:14

# Apply migrations
dotnet ef database update
```

4. **Start Redis**
```bash
docker run --name todo-redis -p 6379:6379 -d redis:7-alpine
```

5. **Run the application**
```bash
dotnet run
# API: https://localhost:5001
# Swagger: https://localhost:5001/swagger
```

### Docker Deployment

#### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TodoApi.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoApi.dll"]
```

#### Docker Compose
```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - JWT_SECRET_KEY=${JWT_SECRET_KEY}
      - DATABASE_CONNECTION_STRING=Host=postgres;Database=todo_api;Username=postgres;Password=${DB_PASSWORD}
      - REDIS_CONNECTION_STRING=redis:6379
    depends_on:
      - postgres
      - redis
    networks:
      - todo-network

  postgres:
    image: postgres:14-alpine
    environment:
      - POSTGRES_PASSWORD=${DB_PASSWORD}
      - POSTGRES_DB=todo_api
    volumes:
      - postgres-data:/var/lib/postgresql/data
    networks:
      - todo-network

  redis:
    image: redis:7-alpine
    networks:
      - todo-network

volumes:
  postgres-data:

networks:
  todo-network:
```

### Kubernetes Deployment

#### Deployment YAML
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: todo-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: todo-api
  template:
    metadata:
      labels:
        app: todo-api
    spec:
      containers:
      - name: api
        image: your-registry/todo-api:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: JWT_SECRET_KEY
          valueFrom:
            secretKeyRef:
              name: todo-secrets
              key: jwt-secret
        - name: DATABASE_CONNECTION_STRING
          valueFrom:
            secretKeyRef:
              name: todo-secrets
              key: db-connection
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health/live
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 10
          periodSeconds: 5
```

---

## 📊 Monitoring & Observability

### Health Checks
```csharp
// Configured in Program.cs
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "database")
    .AddRedis(redisConnection, name: "redis");

// Endpoints
// GET /health          - Detailed health check
// GET /health/live     - Liveness probe (Kubernetes)
```

### Structured Logging with Serilog
```csharp
Log.Information("JWT token created for user {UserId} (Username: {Username})", 
    user.Id, user.Username);

Log.Error(exception, "Failed to create token for user {UserId}", user.Id);
```

### OpenTelemetry Integration
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation());
```

### Application Insights (Azure)
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore

# appsettings.json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-key"
  }
}
```

---

## ⚡ Performance & Scalability

### Caching Strategy
```csharp
// Distributed caching with Redis
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379";
    options.InstanceName = "TodoApi_";
});

// Usage in handlers
var cachedData = await _cache.GetStringAsync("tasks:user:123");
```

### Database Optimization
```csharp
// Connection pooling
builder.Services.AddDbContext<TodoContext>(options =>
{
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.EnableRetryOnFailure(maxRetryCount: 5);
        npgsql.CommandTimeout(30);
    });
});

// Indexes on frequently queried columns
modelBuilder.Entity<TaskItem>()
    .HasIndex(t => t.UserId);
```

### API Rate Limiting
```csharp
// Install: AspNetCoreRateLimit
services.AddMemoryCache();
services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new() { Endpoint = "*", Limit = 100, Period = "1m" }
    };
});
```

---

## 🧪 Testing Strategy

### Unit Tests
```bash
cd tests/TodoApi.UnitTests
dotnet test
```

### Integration Tests
```bash
cd tests/TodoApi.IntegrationTests
dotnet test
```

### Load Testing
```bash
# Using k6
k6 run --vus 100 --duration 30s load-test.js
```

---

## 📝 API Documentation

- **Swagger UI**: `https://localhost:5001/swagger`
- **Health Check**: `https://localhost:5001/health`
- **API Versioning**: `/api/v1/tasks`, `/api/v2/tasks`

---

## 🔄 CI/CD Pipeline

### GitHub Actions Example
```yaml
name: CI/CD

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore -c Release
    - name: Test
      run: dotnet test --no-build --verbosity normal
    - name: Publish
      run: dotnet publish -c Release -o ./publish
    - name: Docker build and push
      run: |
        docker build -t your-registry/todo-api:${{ github.sha }} .
        docker push your-registry/todo-api:${{ github.sha }}
```

---

## 🎓 Best Practices Summary

✅ **Use IOptions<T> for configuration** - Strongly-typed, validated settings  
✅ **Validate configuration at startup** - Fail-fast pattern  
✅ **Never commit secrets** - Use environment variables or Key Vault  
✅ **Implement structured logging** - Serilog with correlation IDs  
✅ **Use UTC timestamps** - Avoid timezone issues  
✅ **Implement health checks** - For Kubernetes liveness/readiness  
✅ **Version your API** - Support backward compatibility  
✅ **Use CQRS pattern** - Separation of reads and writes  
✅ **Implement caching** - Redis for distributed scenarios  
✅ **Monitor everything** - OpenTelemetry + Application Insights  
✅ **Write tests** - Unit, integration, and load tests  
✅ **Document your API** - Swagger with XML comments  

---

## 📞 Support & Contribution

For issues, questions, or contributions, please refer to the project repository.

**Built with ❤️ using Clean Architecture and Enterprise Patterns**
