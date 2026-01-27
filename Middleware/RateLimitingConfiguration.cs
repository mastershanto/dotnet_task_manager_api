using Microsoft.AspNetCore.RateLimiting;
using System.Globalization;
using System.Threading.RateLimiting;

namespace TodoApi.Middleware;

/// <summary>
/// Enterprise-grade rate limiting configuration for API protection.
/// Implements multiple rate limiting strategies to prevent abuse and ensure fair usage.
/// </summary>
public static class RateLimitingConfiguration
{
    /// <summary>
    /// Configures ASP.NET Core built-in rate limiting with multiple policies.
    /// </summary>
    public static IServiceCollection AddEnterpriseRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Global rate limiter rejection response
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.Headers.Add("Retry-After", 
                    context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) 
                        ? ((int)retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture) 
                        : "60");

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "TooManyRequests",
                    message = "Rate limit exceeded. Please try again later.",
                    retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry) 
                        ? (int)retry.TotalSeconds 
                        : 60
                }, cancellationToken: cancellationToken);
            };

            // ============================================
            // Policy 1: Fixed Window - General API
            // ============================================
            // 100 requests per minute per IP
            options.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.PermitLimit = 100;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 10;
            });

            // ============================================
            // Policy 2: Sliding Window - Authenticated Users
            // ============================================
            // 200 requests per minute (sliding window for smoother limits)
            options.AddSlidingWindowLimiter("sliding", opt =>
            {
                opt.PermitLimit = 200;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.SegmentsPerWindow = 4; // 15-second segments
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 20;
            });

            // ============================================
            // Policy 3: Token Bucket - Burst Protection
            // ============================================
            // Allows bursts but maintains average rate
            options.AddTokenBucketLimiter("token", opt =>
            {
                opt.TokenLimit = 50;
                opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                opt.TokensPerPeriod = 10;
                opt.QueueLimit = 5;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            // ============================================
            // Policy 4: Concurrency - Resource Protection
            // ============================================
            // Maximum 20 concurrent requests per user
            options.AddConcurrencyLimiter("concurrency", opt =>
            {
                opt.PermitLimit = 20;
                opt.QueueLimit = 10;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            // ============================================
            // Policy 5: Strict - Authentication Endpoints
            // ============================================
            // Very strict limits for login/register to prevent brute force
            options.AddFixedWindowLimiter("auth", opt =>
            {
                opt.PermitLimit = 5;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0; // No queue for auth endpoints
            });

            // ============================================
            // Policy 6: Per User - Authenticated Endpoints
            // ============================================
            // Rate limit by authenticated user ID
            options.AddPolicy("per-user", context =>
            {
                var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: userId ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 300,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 30
                    });
            });

            // ============================================
            // Policy 7: Per IP - Anonymous Endpoints
            // ============================================
            // Rate limit by IP address for public endpoints
            options.AddPolicy("per-ip", context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ipAddress,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 50,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 5
                    });
            });
        });

        return services;
    }
}

/// <summary>
/// Extension methods for applying rate limiting to endpoints.
/// </summary>
public static class RateLimitingEndpointExtensions
{
    /// <summary>
    /// Disables rate limiting for specific endpoint (use sparingly).
    /// </summary>
    public static TBuilder DisableRateLimiting<TBuilder>(this TBuilder builder) 
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(new DisableRateLimitingAttribute());
    }

    /// <summary>
    /// Applies custom rate limiting policy to endpoint.
    /// </summary>
    public static TBuilder RequireRateLimiting<TBuilder>(this TBuilder builder, string policyName) 
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(new EnableRateLimitingAttribute(policyName));
    }
}
