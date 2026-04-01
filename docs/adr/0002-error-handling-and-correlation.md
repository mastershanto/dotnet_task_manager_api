# ADR 0002: Global Exception Handling And Correlation ID Middleware

## Status
Accepted

## Context
Production APIs need predictable error contracts, traceability, and easier root-cause analysis across logs and downstream systems.

## Decision
Implement global middleware in `shared/http`:

- `ExceptionHandlingMiddleware` to convert unhandled exceptions to `ProblemDetails` (HTTP 500)
- `CorrelationIdMiddleware` to ingest/generate `X-Correlation-ID`, set trace identifier, and return it in response headers

## Consequences
- Pros:
  - consistent failure payloads
  - improved observability and incident triage
  - correlation-id propagation standardizes request tracing
- Trade-offs:
  - additional middleware order sensitivity
  - needs downstream adoption for full distributed tracing value
