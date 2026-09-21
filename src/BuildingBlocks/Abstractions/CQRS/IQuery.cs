using BuildingBlocks.Abstractions;
using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// CQRS Query interface (Read Side):
/// Represents an operation that reads data without modifying state.
/// </summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
