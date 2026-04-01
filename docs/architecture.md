# Architecture Overview

## System Style

This project follows a **modular monolith** architecture with feature-first organization and explicit layering per feature.

## Module Boundaries

- `src/modules/auth`
- `src/modules/user_data`
- `src/modules/product`
- `src/modules/payment`

Each module has:

- `domain`: entities/contracts/interfaces
- `application`: business workflows
- `data`: storage/adapter implementations
- `presentation`: HTTP endpoint registration

## Cross-Cutting Concerns

Located in `shared`:

- `Result<T>` for uniform operation outcomes
- `Validation` for DataAnnotations-based validation
- `Http` middleware:
  - correlation id propagation (`X-Correlation-ID`)
  - global exception handling -> `ProblemDetails`
- `Security` constants for authorization policy names

## API Composition

- Startup and wiring live in `src/Api`.
- DI registration is centralized in `Api/Configuration/DependencyInjection.cs`.
- Endpoint composition is centralized in `Api/Configuration/EndpointMapping.cs`.
- Public API is versioned under `/api/v1`.
- Legacy route mapping is retained for backward compatibility.

## Security Architecture

- JWT bearer authentication with configurable issuer/audience/signing key.
- Authorization policies:
  - `ApiUser`: any authenticated principal
  - `AdminOnly`: authenticated principal with `admin` role claim
- Endpoint policy model:
  - auth login and health endpoints are anonymous
  - user/product endpoints require `ApiUser`
  - payment endpoint requires `AdminOnly`

## Persistence Architecture

- Config-driven provider selection through `Persistence:Provider`:
  - `InMemory` for local/testing flows
  - `Postgres` for production flows
- PostgreSQL adapters:
  - `PostgresUserRepository`
  - `PostgresProductRepository`
  - `PostgresPaymentService`
- Startup migration runner applies ordered SQL files from `infra/postgres/migrations` and records executions in `schema_migrations`.

## Reliability and Operability

- Health endpoints:
  - liveness: `/health/live`
  - readiness: `/health/ready`
- Structured HTTP request logging with duration
- Correlation-id included in response headers and logging scope
- OpenTelemetry tracing + metrics:
  - ASP.NET Core instrumentation
  - HTTP client instrumentation
  - runtime metrics instrumentation
  - optional OTLP exporter

## Testing Model

- Integration tests verify endpoint behavior and middleware contracts.
- Unit tests validate shared primitives and business invariants.
- CI pipeline runs restore/build/test with coverage collection.

## Scalability Notes

Current architecture is prepared for production with PostgreSQL persistence and migration automation.
For higher scale and enterprise growth, next priorities are:

- transactional boundaries per use-case
- optimistic concurrency controls
- cache + outbox patterns where required
- asynchronous messaging for inter-module integration
