using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Categories.Domain;

namespace Categories.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, CategoryModel>
{
    private readonly ICategoryRepository _repository;

    public UpdateCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CategoryModel>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new CategoryModel
        {
            Id = request.Id,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Color = request.Color?.Trim() ?? string.Empty,
            Icon = request.Icon?.Trim() ?? string.Empty
        };

        var updated = await _repository.UpdateAsync(request.Id, category);
        return updated is not null
            ? Result<CategoryModel>.Success(updated)
            : Result<CategoryModel>.Failure($"Category with ID '{request.Id}' was not found.");
    }
}
