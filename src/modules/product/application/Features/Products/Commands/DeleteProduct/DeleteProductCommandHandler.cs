using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Products.Domain;

namespace Products.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand>
{
    private readonly IProductRepository _repository;

    public DeleteProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(request.Id);
        return deleted
            ? Result<bool>.Success(true)
            : Result<bool>.Failure($"Product with ID '{request.Id}' was not found.");
    }
}
