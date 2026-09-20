using Categories.Domain;
using BuildingBlocks.Abstractions;

namespace Categories.Application;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IEnumerable<CategoryModel>>> GetCategoriesAsync() =>
        Result<IEnumerable<CategoryModel>>.Success(await _repo.ListAsync());

    public async Task<Result<CategoryModel>> GetCategoryAsync(Guid id)
    {
        var category = await _repo.GetAsync(id);
        return category is null ? Result<CategoryModel>.Failure("Not found") : Result<CategoryModel>.Success(category);
    }

    public async Task<Result<CategoryModel>> CreateCategoryAsync(CategoryModel category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
            return Result<CategoryModel>.Failure("Name required");

        var created = await _repo.CreateAsync(category);
        return Result<CategoryModel>.Success(created);
    }

    public async Task<Result<CategoryModel>> UpdateCategoryAsync(Guid id, CategoryModel category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
            return Result<CategoryModel>.Failure("Name required");

        var updated = await _repo.UpdateAsync(id, category);
        return updated is null ? Result<CategoryModel>.Failure("Not found") : Result<CategoryModel>.Success(updated);
    }

    public async Task<Result<bool>> DeleteCategoryAsync(Guid id)
    {
        var deleted = await _repo.DeleteAsync(id);
        return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Not found");
    }
}
