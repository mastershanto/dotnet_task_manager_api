using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Products.Domain;

namespace Products.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, IEnumerable<ProductModel>>
{
    private readonly IProductRepository _repository;

    public GetProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<ProductModel>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.ListAsync();
        return Result<IEnumerable<ProductModel>>.Success(products);
    }
}
