using BuildingBlocks.Abstractions;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviors;

/// <summary>
/// MediatR Pipeline Behavior for Cross-Cutting Validation:
/// Executes FluentValidation validators automatically before any Command or Query runs.
/// If validation fails, it returns a failed Result<T> without executing the handler.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => f.ErrorMessage)
            .ToArray();

        if (failures.Length != 0)
        {
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var failureMethod = typeof(TResponse).GetMethod("Failure", new[] { typeof(string[]) });
                if (failureMethod != null)
                {
                    return (TResponse)failureMethod.Invoke(null, new object[] { failures })!;
                }
            }
            throw new ValidationException(validationResults.SelectMany(r => r.Errors));
        }

        return await next();
    }
}
