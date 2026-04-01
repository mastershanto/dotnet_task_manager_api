# Dotnet Task Manager API

Production-oriented modular monolith built with ASP.NET Core Minimal APIs (.NET 10).

## Architecture At A Glance

- Modular feature folders: `auth`, `user_data`, `product`, `payment`
- Layered boundaries per feature:
  - `domain` for contracts/models
  - `application` for use-cases/services
  - `data` for repository/infrastructure adapters
  - `presentation` for endpoint mapping
- Shared cross-cutting concerns in `shared`:
  - result contract (`Result<T>`)
  - model validation helpers
  - HTTP middleware (correlation-id + exception handling)
- Versioned endpoint root: `/api/v1/*`
- Backward-compatible legacy routes remain mapped for transition

## Production Readiness Features

- Centralized unhandled exception middleware returning RFC7807-style `ProblemDetails`
- Correlation ID propagation (`X-Correlation-ID`) for distributed tracing
- HTTP logging with path/method/status/duration
- Liveness and readiness probes:
  - `GET /health/live`
  - `GET /health/ready`
- OpenAPI/Swagger for development discovery

## Run Locally

```bash
dotnet restore src/Api/Api.csproj
dotnet restore src/tests/Api.Tests.csproj
dotnet build src/Api/Api.csproj --configuration Release
dotnet test src/tests/Api.Tests.csproj --configuration Release
dotnet run --project src/Api/Api.csproj
```

Swagger UI: `http://localhost:5000/swagger` (port may vary by profile).

## API Surface (v1)

- `POST /api/v1/auth/login`
- `GET /api/v1/users`
- `GET /api/v1/users/{id}`
- `POST /api/v1/users`
- `PUT /api/v1/users/{id}`
- `DELETE /api/v1/users/{id}`
- `GET /api/v1/products`
- `GET /api/v1/products/{id}`
- `POST /api/v1/products`
- `PUT /api/v1/products/{id}`
- `DELETE /api/v1/products/{id}`
- `POST /api/v1/payment/process`

## Test Strategy

- `src/tests` contains:
  - API integration tests (status codes, health probes, validation behavior)
  - focused unit tests for validation/result contracts
- CI pipeline executes restore/build/test and collects coverage.

## Next Enterprise Upgrades

- Replace in-memory data adapters with PostgreSQL persistence + migrations
- Add authN/authZ policy enforcement
- Add OpenTelemetry traces/metrics export
- Add contract tests and consumer-driven API tests
- Add deployment manifests and environment-specific configuration matrix
