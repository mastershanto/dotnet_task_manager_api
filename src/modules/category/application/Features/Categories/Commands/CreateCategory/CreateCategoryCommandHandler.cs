using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Categories.Domain;

namespace Categories.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, CategoryModel>
{
    private readonly ICategoryRepository _repository;

    public CreateCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CategoryModel>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new CategoryModel
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Color = request.Color?.Trim() ?? string.Empty,
            Icon = request.Icon?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(category);
        return Result<CategoryModel>.Success(created);
    }
}
