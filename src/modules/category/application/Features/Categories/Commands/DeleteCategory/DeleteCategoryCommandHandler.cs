using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Categories.Domain;

namespace Categories.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _repository;

    public DeleteCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(request.Id);
        return deleted
            ? Result<bool>.Success(true)
            : Result<bool>.Failure($"Category with ID '{request.Id}' was not found.");
    }
}
