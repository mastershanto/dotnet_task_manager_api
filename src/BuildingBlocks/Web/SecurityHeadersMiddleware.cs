using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web;

/// <summary>
/// সিকিউরিটি হেডার্স মিডলওয়্যার (Security Headers Middleware):
/// OWASP সিকিউরিটি বেস্ট প্র্যাকটিস অনুযায়ী রেসপন্সে প্রয়োজনীয় সিকিউরিটি হেডার যুক্ত করে:
/// - X-Content-Type-Options: nosniff
/// - X-Frame-Options: DENY
/// - X-XSS-Protection: 1; mode=block
/// - Referrer-Policy: strict-origin-when-cross-origin
/// </summary>
public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            if (!headers.ContainsKey("X-Content-Type-Options"))
                headers["X-Content-Type-Options"] = "nosniff";

            if (!headers.ContainsKey("X-Frame-Options"))
                headers["X-Frame-Options"] = "DENY";

            if (!headers.ContainsKey("X-XSS-Protection"))
                headers["X-XSS-Protection"] = "1; mode=block";

            if (!headers.ContainsKey("Referrer-Policy"))
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            return Task.CompletedTask;
        });

        await _next(context);
    }
}
