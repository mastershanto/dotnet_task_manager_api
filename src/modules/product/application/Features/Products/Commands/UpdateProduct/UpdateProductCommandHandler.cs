using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Products.Domain;

namespace Products.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, ProductModel>
{
    private readonly IProductRepository _repository;

    public UpdateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProductModel>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new ProductModel
        {
            Id = request.Id,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Price = request.Price,
            Category = request.Category?.Trim() ?? string.Empty
        };

        var updated = await _repository.UpdateAsync(request.Id, product);
        return updated is not null
            ? Result<ProductModel>.Success(updated)
            : Result<ProductModel>.Failure($"Product with ID '{request.Id}' was not found.");
    }
}
