# ============================================
# ENTERPRISE MULTI-STAGE DOCKERFILE
# ============================================
# Optimized for production with security and performance best practices

# ============================================
# Stage 1: Base Runtime Image
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

# Security: Run as non-root user
RUN addgroup -g 1000 appgroup && \
    adduser -D -u 1000 -G appgroup appuser && \
    chown -R appuser:appgroup /app

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# ============================================
# Stage 2: Build Image
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy project file and restore dependencies (layer caching optimization)
COPY ["TodoApi.csproj", "./"]
RUN dotnet restore "TodoApi.csproj" \
    --runtime linux-musl-x64 \
    --locked-mode

# Copy source code and build
COPY . .
RUN dotnet build "TodoApi.csproj" \
    -c Release \
    -o /app/build \
    --no-restore \
    --runtime linux-musl-x64 \
    /p:TreatWarningsAsErrors=true

# ============================================
# Stage 3: Publish Image
# ============================================
FROM build AS publish
RUN dotnet publish "TodoApi.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    --no-build \
    --runtime linux-musl-x64 \
    --self-contained false \
    /p:PublishTrimmed=false \
    /p:PublishSingleFile=false

# ============================================
# Stage 4: Final Runtime Image
# ============================================
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish --chown=appuser:appgroup /app/publish .

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health/live || exit 1

# Entry point
ENTRYPOINT ["dotnet", "TodoApi.dll"]
