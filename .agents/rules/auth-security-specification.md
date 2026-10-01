---
trigger: always_on
---

# Authentication & Security Specification (World-Class Clean Architecture + CQRS)

## 1. Architectural Integrity
- Follow Clean Architecture and CQRS strictly:
  - **Domain**: Pure models (`UserModel`, `OtpCodeModel`), repository contracts (`IAuthRepository`), domain services interfaces. Zero framework dependencies.
  - **Application**: CQRS Commands, Queries, Handlers (MediatR `IRequestHandler`), FluentValidation validators (`AbstractValidator<T>`).
  - **Infrastructure/Data**: EF Core repository implementation (`EfAuthRepository`), `JwtTokenService`, `PasswordHasherService`, `OtpService`.
  - **Presentation**: Minimal APIs (`AuthEndpoints.cs`) delegating execution strictly to MediatR `ISender`.

## 2. Authentication & Account Lifecycle Endpoints
- **Sign Up / Register**: `POST /api/v1/auth/register`
  - Validates name, email, password strength.
  - Hashes password using secure PBKDF2 / ASP.NET Core PasswordHasher.
  - Creates unverified user and generates 6-digit OTP (10 mins expiration).
  - *Returns generated OTP in the API response* for development/testing until SMTP provider is hooked up.
- **Verify Registration OTP**: `POST /api/v1/auth/verify-registration-otp`
  - Verifies OTP, marks `IsEmailVerified = true`, marks OTP used.
  - Issues JWT Bearer token and returns authenticated user details.
- **Login**: `POST /api/v1/auth/login`
  - Validates email and password against hash.
  - Checks if email is verified.
  - Issues JWT Bearer token with claims: `sub`, `email`, `name`, `role`.
- **Forgot Password**: `POST /api/v1/auth/forgot-password`
  - Validates user existence.
  - Generates 6-digit OTP for password reset.
  - *Returns generated OTP in response* for testing until SMTP is active.
- **Verify Reset OTP**: `POST /api/v1/auth/verify-reset-otp`
  - Validates whether the OTP is active and matches.
- **Reset Password**: `POST /api/v1/auth/reset-password`
  - Validates OTP and updates password hash.
- **Profile (Authorized - `AuthPolicies.ApiUser`)**:
  - `GET /api/v1/auth/profile`: Returns current authenticated user's profile.
  - `PUT /api/v1/auth/profile`: Edits user profile (name, etc.).
  - `POST /api/v1/auth/change-password`: Validates old password, hashes and updates new password.
  - `DELETE /api/v1/auth/account`: Confirms password and deletes account.
  - `POST /api/v1/auth/logout`: Revokes/terminates session.

## 3. Middleware & Security Standards
- **ExceptionHandlingMiddleware**: Catches FluentValidation exceptions (returns 400 Bad Request ProblemDetails) and unexpected server errors (500).
- **CorrelationIdMiddleware**: Propagates `X-Correlation-Id` across all requests.
- **SecurityHeadersMiddleware**: Sets `X-Content-Type-Options`, `X-Frame-Options`, `X-XSS-Protection`, `Referrer-Policy`.
- **Swagger UI**: Configured with JWT Bearer Authentication scheme ("Authorize" button).
