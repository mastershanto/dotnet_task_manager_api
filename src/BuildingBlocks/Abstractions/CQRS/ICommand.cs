using BuildingBlocks.Abstractions;
using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// CQRS Command interface with return value:
/// Represent an operation that modifies state (Write Side).
/// </summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

/// <summary>
/// CQRS Command interface without return value (returns Result<bool>).
/// </summary>
public interface ICommand : IRequest<Result<bool>>
{
}
