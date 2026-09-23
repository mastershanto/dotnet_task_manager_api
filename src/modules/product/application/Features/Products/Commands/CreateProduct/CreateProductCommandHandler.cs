using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Products.Domain;

namespace Products.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductModel>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProductModel>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Price = request.Price,
            Category = request.Category?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(product);
        return Result<ProductModel>.Success(created);
    }
}
