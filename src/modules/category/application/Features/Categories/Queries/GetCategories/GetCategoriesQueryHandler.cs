using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Categories.Domain;

namespace Categories.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, IEnumerable<CategoryModel>>
{
    private readonly ICategoryRepository _repository;

    public GetCategoriesQueryHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<CategoryModel>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _repository.ListAsync();
        return Result<IEnumerable<CategoryModel>>.Success(categories);
    }
}
