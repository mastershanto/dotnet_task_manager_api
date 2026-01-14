using System.Text.Json;
using TodoApi.DTOs;

namespace TodoApi.Middleware;

public class ExceptionHandlingMiddleware
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);
            await HandleException(context, exception);
        }
    }

    private static Task HandleException(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new ApiResponse<object>();

        switch (exception)
        {
            case ArgumentNullException argNullEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Success = false;
                response.Message = "Invalid input provided";
                response.Errors.Add(argNullEx.ParamName ?? "Unknown parameter");
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Success = false;
                response.Message = "Invalid input provided";
                response.Errors.Add(argEx.Message);
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Success = false;
                response.Message = "Unauthorized access";
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response.Success = false;
                response.Message = "Resource not found";
                break;

            case InvalidOperationException invOpEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Success = false;
                response.Message = "Invalid operation";
                response.Errors.Add(invOpEx.Message);
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Success = false;
                response.Message = "An internal server error has occurred";
                response.Errors.Add(exception.Message);
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
