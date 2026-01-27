using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace TodoApi.Services;

/// <summary>
/// Enterprise-grade JWT authentication configuration with security best practices.
/// </summary>
public static class AuthExtensions
{
    /// <summary>
    /// Configures JWT Bearer authentication with enterprise security features:
    /// - Strongly-typed configuration with IOptions pattern
    /// - Configurable clock skew for distributed systems
    /// - Security event logging for audit trails
    /// - Proper token validation and error handling
    /// - Role-based authorization support
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Get validated JWT settings from IOptions (already validated in Program.cs)
        var sp = services.BuildServiceProvider();
        var jwtSettings = sp.GetRequiredService<IOptions<JwtSettings>>().Value;

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Security settings
                options.RequireHttpsMetadata = true; // Require HTTPS in production
                options.SaveToken = true; // Save token in AuthenticationProperties
                
                // Token validation parameters
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Validate the token issuer
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    
                    // Validate the token audience
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    
                    // Validate the signing key
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    
                    // Validate token lifetime
                    ValidateLifetime = true,
                    
                    // Clock skew compensation for distributed systems
                    // Allows tokens to be accepted slightly before/after exact time
                    ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkewSeconds),
                    
                    // Require expiration time
                    RequireExpirationTime = true,
                    
                    // Additional security validations
                    RequireSignedTokens = true
                };
                
                // Security event handlers for logging and monitoring
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Log.Warning("JWT authentication failed for {Path}: {Error}", 
                            context.Request.Path, 
                            context.Exception.Message);
                        
                        // Add custom error response
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        
                        return Task.CompletedTask;
                    },
                    
                    OnTokenValidated = context =>
                    {
                        var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        Log.Debug("Token validated successfully for user {UserId}", userId);
                        return Task.CompletedTask;
                    },
                    
                    OnChallenge = context =>
                    {
                        Log.Warning("JWT authentication challenge for {Path}: {Error}", 
                            context.Request.Path, 
                            context.ErrorDescription ?? "Unauthorized");
                        return Task.CompletedTask;
                    },
                    
                    OnMessageReceived = context =>
                    {
                        // Support token from query string (for SignalR/WebSockets)
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        
                        if (!string.IsNullOrEmpty(accessToken) && 
                            path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        
                        return Task.CompletedTask;
                    }
                };
            });

        // Authorization policies
        services.AddAuthorization(options =>
        {
            // Default policy requires authenticated user
            options.FallbackPolicy = options.DefaultPolicy;
            
            // Admin-only policy
            options.AddPolicy("AdminOnly", policy => 
                policy.RequireRole("Admin"));
            
            // Manager or Admin policy
            options.AddPolicy("ManagerOrAdmin", policy =>
                policy.RequireRole("Manager", "Admin"));
        });

        return services;
    }
}
