# Architecture Overview

## System Style

This project follows a **modular monolith** architecture with feature-first organization and explicit layering per feature.

## Module Boundaries

- `auth`
- `user_data`
- `product`
- `payment`

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

## API Composition

- Startup and wiring live in `src/Api`.
- DI registration is centralized in `Api/Configuration/DependencyInjection.cs`.
- Endpoint composition is centralized in `Api/Configuration/EndpointMapping.cs`.
- Public API is versioned under `/api/v1`.
- Legacy route mapping is retained for backward compatibility.

## Reliability and Operability

- Health endpoints:
  - liveness: `/health/live`
  - readiness: `/health/ready`
- Structured HTTP request logging with duration
- Correlation-id included in response headers and logging scope

## Testing Model

- Integration tests verify endpoint behavior and middleware contracts.
- Unit tests validate shared primitives and business invariants.
- CI pipeline runs restore/build/test with coverage collection.

## Scalability Notes

Current in-memory repositories are thread-safe and adequate for local/dev use.
For production scale, migrate module data adapters to PostgreSQL and introduce:

- transactional boundaries per use-case
- optimistic concurrency controls
- cache + outbox patterns where required
- asynchronous messaging for inter-module integration
