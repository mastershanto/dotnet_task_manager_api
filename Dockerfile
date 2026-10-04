# ============================================
# PRODUCTION MULTI-STAGE DOCKERFILE
# ============================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app

# Security: Run as non-root user
RUN addgroup -g 1000 appgroup && \
    adduser -D -u 1000 -G appgroup appuser && \
    chown -R appuser:appgroup /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy all source files and restore solution
COPY . .
RUN dotnet restore "dotnet_task_manager_api.sln"

RUN dotnet publish "src/Api/Api.csproj" \
    -c Release \
    -o /app/publish \
    --self-contained false \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build --chown=appuser:appgroup /app/publish .

USER appuser

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "Api.dll"]
