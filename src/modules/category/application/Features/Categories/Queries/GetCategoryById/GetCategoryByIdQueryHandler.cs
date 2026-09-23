using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Categories.Domain;

namespace Categories.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, CategoryModel?>
{
    private readonly ICategoryRepository _repository;

    public GetCategoryByIdQueryHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CategoryModel?>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetAsync(request.Id);
        return category is not null
            ? Result<CategoryModel?>.Success(category)
            : Result<CategoryModel?>.Failure($"Category with ID '{request.Id}' was not found.");
    }
}
