using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviors;

/// <summary>
/// MediatR Pipeline Behavior for Telemetry and Logging:
/// Logs every CQRS request execution, duration, and flags slow-running operations.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("--> [CQRS Pipeline] Starting {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > 500)
        {
            _logger.LogWarning("--> [CQRS Pipeline] SLOW EXECUTION: {RequestName} finished in {ElapsedMilliseconds} ms", requestName, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            _logger.LogInformation("--> [CQRS Pipeline] Completed {RequestName} in {ElapsedMilliseconds} ms", requestName, stopwatch.ElapsedMilliseconds);
        }

        return response;
    }
}
