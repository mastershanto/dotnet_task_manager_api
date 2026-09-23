using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Products.Domain;

namespace Products.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductModel?>
{
    private readonly IProductRepository _repository;

    public GetProductByIdQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProductModel?>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetAsync(request.Id);
        return product is not null
            ? Result<ProductModel?>.Success(product)
            : Result<ProductModel?>.Failure($"Product with ID '{request.Id}' was not found.");
    }
}
