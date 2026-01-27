using System.Reflection;
using Asp.Versioning;
using FluentValidation;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using TodoApi.Application.Abstractions;
using TodoApi.Application.Common.Behaviors;
using TodoApi.Data;
using TodoApi.Infrastructure.Persistence;
using TodoApi.Infrastructure.Repositories;
using TodoApi.Middleware;
using TodoApi.Services;

// Configure Serilog for enterprise logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/todoapi-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .CreateLogger();

try
{
    Log.Information("Starting Enterprise Todo API");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // API Versioning - Enterprise standard
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    // Add controllers
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // MediatR for CQRS - Enterprise pattern
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    });

    // MediatR Pipeline Behaviors - Enterprise cross-cutting concerns
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

    // FluentValidation - Auto-register all validators
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    // Swagger - Enterprise documentation
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Enterprise Task Manager API",
            Version = "v1.0",
            Description = @"
    🏢 **World-Class Enterprise-Grade Clean Architecture API**
    
    ## Architecture Highlights
    - ✅ Clean Architecture (Domain, Application, Infrastructure, Presentation)
    - ✅ CQRS Pattern with MediatR
    - ✅ Domain-Driven Design (Rich Entities, Value Objects, Domain Events)
    - ✅ Repository Pattern & Unit of Work
    - ✅ Result Pattern for error handling
    - ✅ Comprehensive validation pipeline
    - ✅ Performance monitoring
    - ✅ Structured logging with Serilog
    - ✅ API Versioning
    - ✅ Health Checks
    - ✅ Redis Caching
    - ✅ Resiliency patterns with Polly
    
    ## Enterprise Features
    - JWT Authentication & Authorization
    - Soft Delete support
    - Audit trails
    - Domain events for decoupled architecture
    - Specification pattern for complex queries
    - Global exception handling
    ",
            Contact = new OpenApiContact
            {
                Name = "Enterprise Development Team",
                Email = "dev@enterprise.com"
            }
        });

        // JWT Security Definition
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Include XML comments if available
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    // CORS - Enterprise configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Database - Entity Framework Core
    builder.Services.AddDbContext<TodoContext>(options =>
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection") ??
            "Host=localhost;Database=todo_api;Username=postgres;Password=password",
            npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
    });

    // Redis Caching - Enterprise caching strategy
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
        options.InstanceName = "TodoApi_";
    });

    // Health Checks - Enterprise monitoring
    builder.Services.AddHealthChecks()
        .AddNpgSql(
            builder.Configuration.GetConnectionString("DefaultConnection") ??
            "Host=localhost;Database=todo_api;Username=postgres;Password=password",
            name: "database")
        .AddRedis(
            builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379",
            name: "redis");

    // OpenTelemetry - Tracing & Metrics (enterprise observability)
    var otelSection = builder.Configuration.GetSection("OpenTelemetry");
    var otelServiceName = otelSection["ServiceName"] ?? "TodoApi";
    var otelOtlpEndpoint = otelSection["OtlpEndpoint"];
    var otelEnableConsoleExporter = string.Equals(otelSection["ConsoleExporterEnabled"], "true", StringComparison.OrdinalIgnoreCase);

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService(serviceName: otelServiceName))
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

            if (!string.IsNullOrWhiteSpace(otelOtlpEndpoint))
            {
                tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otelOtlpEndpoint));
            }

            if (otelEnableConsoleExporter)
            {
                tracing.AddConsoleExporter();
            }
        })
        .WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation();

            if (!string.IsNullOrWhiteSpace(otelOtlpEndpoint))
            {
                metrics.AddOtlpExporter(o => o.Endpoint = new Uri(otelOtlpEndpoint));
            }
        });

    // Response Compression - Enterprise performance optimization
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
        options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
    });

    builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
    {
        options.Level = System.IO.Compression.CompressionLevel.Optimal;
    });

    builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
    {
        options.Level = System.IO.Compression.CompressionLevel.Optimal;
    });

    // Rate Limiting - Enterprise API protection
    builder.Services.AddEnterpriseRateLimiting();

            if (!string.IsNullOrWhiteSpace(otelOtlpEndpoint))
            {
                tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otelOtlpEndpoint));
            }

            if (otelEnableConsoleExporter)
            {
                tracing.AddConsoleExporter();
            }
        })
        .WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation();

            if (!string.IsNullOrWhiteSpace(otelOtlpEndpoint))
            {
                metrics.AddOtlpExporter(o => o.Endpoint = new Uri(otelOtlpEndpoint));
            }
        });

    // JWT Configuration - Enterprise strongly-typed settings with validation
    var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName);
    builder.Services.Configure<JwtSettings>(jwtSection);

    // Validate JWT settings at startup (fail-fast pattern)
    var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();
    try
    {
        jwtSettings.Validate();
        Log.Information("✅ JWT configuration validated successfully");
    }
    catch (InvalidOperationException ex)
    {
        Log.Fatal(ex, "❌ JWT configuration validation failed. Check appsettings.json");
        throw;
    }

    // Authentication
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // Infrastructure - Repository & Unit of Work pattern
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<ITaskRepository, TaskRepository>();
    builder.Services.AddScoped<IProjectAccessService, ProjectAccessService>();
    builder.Services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();

    // NOTE: Legacy service layer - use CQRS handlers instead
    // MediatR handlers in Application/Features/Tasks will handle all business logic
    // This service may be kept for backward compatibility but new features should use handlers

    var app = builder.Build();

    // Global exception handling middleware
    app.UseExceptionHandling();

    // Security headers middleware - Enterprise security
    app.UseSecurityHeaders();

    // Response compression - Performance optimization
    app.UseResponseCompression();

    // Swagger UI
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Enterprise Task Manager API v1");
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });
    }

    // Health checks endpoint
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    // Basic health check
    app.MapGet("/health/live", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }))
        .WithTags("Health");

    if (!app.Environment.IsEnvironment("Testing"))
    {
        app.UseHttpsRedirection();
    }
    
    app.UseCors("AllowAll");

    // Rate limiting - Must be before authentication
    app.UseRateLimiter();

    // Logging middleware
    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Apply migrations on startup
    if (!app.Environment.IsEnvironment("Testing"))
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TodoContext>();
            await db.Database.MigrateAsync();
            Log.Information("✅ Database initialized successfully");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "⚠️ Could not create/initialize database");
        }
    }

    Log.Information("🚀 Enterprise Todo API started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
}

public partial class Program { }
