using Microsoft.Extensions.Primitives;

namespace TodoApi.Middleware;

/// <summary>
/// Enterprise-grade security headers middleware implementing OWASP security best practices.
/// Adds comprehensive security headers to protect against common web vulnerabilities.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;
    private readonly SecurityHeadersOptions _options;

    public SecurityHeadersMiddleware(
        RequestDelegate next, 
        ILogger<SecurityHeadersMiddleware> logger,
        SecurityHeadersOptions? options = null)
    {
        _next = next;
        _logger = logger;
        _options = options ?? new SecurityHeadersOptions();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers before processing the request
        AddSecurityHeaders(context.Response.Headers);

        await _next(context);
    }

    private void AddSecurityHeaders(IHeaderDictionary headers)
    {
        // X-Content-Type-Options: Prevents MIME-sniffing attacks
        if (_options.AddXContentTypeOptions)
        {
            headers["X-Content-Type-Options"] = "nosniff";
        }

        // X-Frame-Options: Prevents clickjacking attacks
        if (_options.AddXFrameOptions)
        {
            headers["X-Frame-Options"] = _options.XFrameOptionsValue;
        }

        // X-XSS-Protection: Enables browser's XSS filter (legacy browsers)
        if (_options.AddXXssProtection)
        {
            headers["X-XSS-Protection"] = "1; mode=block";
        }

        // Strict-Transport-Security (HSTS): Forces HTTPS
        if (_options.AddStrictTransportSecurity)
        {
            headers["Strict-Transport-Security"] = 
                $"max-age={_options.HstsMaxAge}; includeSubDomains; preload";
        }

        // Content-Security-Policy: Prevents XSS, injection attacks
        if (_options.AddContentSecurityPolicy && !string.IsNullOrWhiteSpace(_options.ContentSecurityPolicy))
        {
            headers["Content-Security-Policy"] = _options.ContentSecurityPolicy;
        }

        // Referrer-Policy: Controls referrer information
        if (_options.AddReferrerPolicy)
        {
            headers["Referrer-Policy"] = _options.ReferrerPolicyValue;
        }

        // Permissions-Policy: Controls browser features and APIs
        if (_options.AddPermissionsPolicy && !string.IsNullOrWhiteSpace(_options.PermissionsPolicy))
        {
            headers["Permissions-Policy"] = _options.PermissionsPolicy;
        }

        // X-Permitted-Cross-Domain-Policies: Restricts Adobe Flash/PDF
        if (_options.AddXPermittedCrossDomainPolicies)
        {
            headers["X-Permitted-Cross-Domain-Policies"] = "none";
        }

        // Remove server information header (security through obscurity)
        if (_options.RemoveServerHeader)
        {
            headers.Remove("Server");
            headers.Remove("X-Powered-By");
        }

        // Cache-Control for sensitive responses
        if (_options.AddCacheControl)
        {
            headers["Cache-Control"] = "no-store, no-cache, must-revalidate, private";
            headers["Pragma"] = "no-cache";
        }
    }
}

/// <summary>
/// Configuration options for security headers.
/// </summary>
public class SecurityHeadersOptions
{
    public bool AddXContentTypeOptions { get; set; } = true;
    public bool AddXFrameOptions { get; set; } = true;
    public string XFrameOptionsValue { get; set; } = "DENY"; // DENY | SAMEORIGIN
    public bool AddXXssProtection { get; set; } = true;
    public bool AddStrictTransportSecurity { get; set; } = true;
    public int HstsMaxAge { get; set; } = 31536000; // 1 year in seconds
    public bool AddContentSecurityPolicy { get; set; } = true;
    public string ContentSecurityPolicy { get; set; } = 
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' data:; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self';";
    public bool AddReferrerPolicy { get; set; } = true;
    public string ReferrerPolicyValue { get; set; } = "strict-origin-when-cross-origin";
    public bool AddPermissionsPolicy { get; set; } = true;
    public string PermissionsPolicy { get; set; } = 
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";
    public bool AddXPermittedCrossDomainPolicies { get; set; } = true;
    public bool RemoveServerHeader { get; set; } = true;
    public bool AddCacheControl { get; set; } = false; // Only for sensitive endpoints
}

/// <summary>
/// Extension methods for adding security headers middleware.
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    /// <summary>
    /// Adds enterprise-grade security headers to the application pipeline.
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(
        this IApplicationBuilder builder, 
        SecurityHeadersOptions? options = null)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>(options ?? new SecurityHeadersOptions());
    }

    /// <summary>
    /// Adds security headers middleware with custom configuration.
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(
        this IApplicationBuilder builder, 
        Action<SecurityHeadersOptions> configure)
    {
        var options = new SecurityHeadersOptions();
        configure(options);
        return builder.UseMiddleware<SecurityHeadersMiddleware>(options);
    }
}
