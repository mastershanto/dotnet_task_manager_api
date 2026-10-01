using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web;

/// <summary>
/// গ্লোবাল এক্সেপশন হ্যান্ডলিং মিডলওয়্যার:
/// ১. ভ্যালিডেশন এরর ঘটলে RFC 7807 ফরম্যাটে 400 Bad Request প্রদান করে।
/// ২. অপ্রত্যাশিত সার্ভার এরর ঘটলে ট্রেস আইডিসহ 500 Internal Server Error প্রদান করে।
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException valEx)
        {
            _logger.LogWarning("Validation failure: {Message}", valEx.Message);

            var errors = valEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var problem = Results.ValidationProblem(
                errors: errors,
                detail: "One or more validation errors occurred.",
                statusCode: StatusCodes.Status400BadRequest);

            await problem.ExecuteAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing {Method} {Path}.", context.Request.Method, context.Request.Path);

            var problem = Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unhandled server error",
                detail: "An unexpected error occurred while processing your request.",
                extensions: new Dictionary<string, object?>
                {
                    ["traceId"] = context.TraceIdentifier
                });

            await problem.ExecuteAsync(context);
        }
    }
}
