# Architecture Overview

## System Style

This project follows a **modular monolith** architecture adhering to **Clean Architecture** principles and the **CQRS (Command Query Responsibility Segregation)** pattern with vertical-slice feature organization.

## Module Boundaries

- `src/modules/task`
- `src/modules/project`
- `src/modules/category`
- `src/modules/product`
- `src/modules/user_data`
- `src/modules/payment`
- `src/modules/auth`

## Clean Architecture & CQRS Layering per Module

Each module strictly follows explicit Clean Architecture layering and the inward dependency rule:

```
[Presentation (Minimal APIs)]
           ↓ (ISender)
[Application (CQRS Commands & Queries, Handlers, Validators)]
           ↓
    [Domain (Core Models, Contracts, Interfaces)]
           ↑
[Data / Infrastructure (EF Core Repositories, DbContext, Configurations)]
```

### 1. `domain`
- Pure domain models (`TaskItemModel`, `ProjectModel`, `CategoryModel`, `ProductModel`, `UserModel`, `PaymentModel`, `AuthResult`).
- Repository and domain service interfaces (`ITaskRepository`, `IProjectRepository`, `ICategoryRepository`, `IProductRepository`, `IUserRepository`, etc.).
- Zero external dependencies on UI or database frameworks.

### 2. `application`
- Structured as vertical feature slices: `Features/<FeatureName>/Commands/` and `Features/<FeatureName>/Queries/`.
- **Commands (Write Side)**: Implements `ICommand<T>` / `ICommand` and `ICommandHandler<TCommand, TResult>`. Changes state, enforces invariants, and persists via repositories.
- **Queries (Read Side)**: Implements `IQuery<T>` and `IQueryHandler<TQuery, TResult>`. Read-optimized data retrieval.
- **Validation**: Declarative rules implemented with FluentValidation (`AbstractValidator<T>`). Executed automatically via MediatR pipeline behaviors prior to handler execution.

### 3. `data`
- EF Core database configurations implementing `IEntityTypeConfiguration<T>`.
- Repository implementations (`EfTaskRepository`, `EfProjectRepository`, `EfCategoryRepository`, `EfProductRepository`, `EfUserRepository`, `EfPaymentService`).
- Execution resilience, query optimization (`AsNoTracking()`), and automatic auditing via `AuditableEntityInterceptor`.

### 4. `presentation`
- High-performance ASP.NET Core Minimal API endpoints with Swagger OpenAPI metadata.
- Endpoints act purely as dispatchers: bind HTTP requests, dispatch commands or queries via MediatR `ISender`, and return standardized HTTP responses.
- Decentralized module dependency injection extension methods (`Add<Module>Module()`, `Map<Module>Endpoints()`).

## Cross-Cutting Concerns & Pipeline Behaviors

Located in `BuildingBlocks` (`BuildingBlocks.Abstractions`, `BuildingBlocks.Persistence`, `BuildingBlocks.Security`):

- **MediatR Pipeline Behaviors**:
  - `LoggingBehavior<TRequest, TResponse>`: Performance tracking and structured logging.
  - `ValidationBehavior<TRequest, TResponse>`: Automatic validation using registered FluentValidation validators.
- **Result Pattern**: `Result<T>` for predictable, exception-free failure/success handling.
- **Security**: Centralized policy definitions (`AuthPolicies.ApiUser`, `AuthPolicies.AdminOnly`).
- **HTTP Middleware**:
  - Correlation ID propagation (`X-Correlation-ID`).
  - Global exception handling producing standard RFC 7807 `ProblemDetails`.

## API Composition

- Startup and host wiring live in `src/Api`.
- DI registration is centralized in `Api/Configuration/DependencyInjection.cs`.
- Endpoint composition is centralized in `Api/Configuration/EndpointMapping.cs`.
- Public API is versioned under `/api/v1`.
- Backward-compatible route mapping preserved for legacy clients.

## Security Architecture

- JWT bearer authentication with configurable issuer/audience/signing key.
- Authorization policies:
  - `ApiUser`: any authenticated principal
  - `AdminOnly`: authenticated principal with `admin` role claim
- Endpoint policy model:
  - auth login and health endpoints are anonymous
  - tasks, projects, categories, users, products require `ApiUser`
  - payment processing requires `AdminOnly`

## Persistence Architecture

- Entity Framework Core 10 unified persistence engine in `BuildingBlocks.Persistence`.
- Config-driven provider selection through `Persistence:Provider`:
  - `InMemory`: EF Core InMemory database for rapid zero-dependency local/testing flows
  - `Postgres`: `Npgsql.EntityFrameworkCore.PostgreSQL` with connection pooling (`AddDbContextPool`) and execution resilience (`EnableRetryOnFailure`) for production
- Centralized `AuditableEntityInterceptor` for automatic UTC timestamp auditing.
- Automatic assembly discovery of entity configurations (`IEntityTypeConfiguration<T>`).
- Startup migration runner applies ordered SQL files from `infra/postgres/migrations` and records executions in `schema_migrations`.

## Reliability and Operability

- Health endpoints:
  - liveness: `/health/live`
  - readiness: `/health/ready`
- Structured HTTP request logging with duration.
- Correlation-ID included in response headers and logging scope.
- OpenTelemetry tracing and metrics instrumentation.

## Testing Model

- **CQRS Handler Tests**: Unit tests verifying commands, queries, and validators in isolation with mock/fake repositories (`TaskCqrsTests`, `ProjectCqrsTests`, `CategoryCqrsTests`, `ProductCqrsTests`, `UserCqrsTests`).
- **Integration Tests**: WebApplicationFactory end-to-end tests validating HTTP status codes, authorization policies, and middleware contracts (`CategoryEndpointsTests`, `UserEndpointsTests`).
