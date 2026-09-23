using Categories.Application.Features.Categories.Commands.CreateCategory;
using Categories.Application.Features.Categories.Commands.DeleteCategory;
using Categories.Application.Features.Categories.Commands.UpdateCategory;
using Categories.Application.Features.Categories.Queries.GetCategories;
using Categories.Application.Features.Categories.Queries.GetCategoryById;
using Categories.Domain;
using Xunit;

namespace Api.Tests;

public class FakeCategoryRepository : ICategoryRepository
{
    private readonly List<CategoryModel> _categories = new();

    public Task<IEnumerable<CategoryModel>> ListAsync() =>
        Task.FromResult<IEnumerable<CategoryModel>>(_categories.ToList());

    public Task<CategoryModel?> GetAsync(Guid id) =>
        Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));

    public Task<CategoryModel> CreateAsync(CategoryModel category)
    {
        _categories.Add(category);
        return Task.FromResult(category);
    }

    public Task<CategoryModel?> UpdateAsync(Guid id, CategoryModel category)
    {
        var idx = _categories.FindIndex(c => c.Id == id);
        if (idx == -1) return Task.FromResult<CategoryModel?>(null);
        _categories[idx] = category;
        return Task.FromResult<CategoryModel?>(category);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var item = _categories.FirstOrDefault(c => c.Id == id);
        if (item is null) return Task.FromResult(false);
        _categories.Remove(item);
        return Task.FromResult(true);
    }
}

public class CategoryCqrsTests
{
    [Fact]
    public async Task CreateCategoryCommand_Success_WhenValid()
    {
        var repo = new FakeCategoryRepository();
        var handler = new CreateCategoryCommandHandler(repo);

        var command = new CreateCategoryCommand("Work", "Tasks for work", "#3B82F6", "briefcase");
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Work", result.Value.Name);
    }

    [Fact]
    public void CreateCategoryValidator_Fails_WhenNameIsTooShort()
    {
        var validator = new CreateCategoryCommandValidator();
        var command = new CreateCategoryCommand("A");

        var validationResult = validator.Validate(command);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(CreateCategoryCommand.Name));
    }

    [Fact]
    public async Task GetCategoriesQuery_ReturnsAll()
    {
        var repo = new FakeCategoryRepository();
        await repo.CreateAsync(new CategoryModel { Name = "Category 1" });
        await repo.CreateAsync(new CategoryModel { Name = "Category 2" });

        var handler = new GetCategoriesQueryHandler(repo);
        var result = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task UpdateCategoryCommand_ReturnsFailure_WhenNotFound()
    {
        var repo = new FakeCategoryRepository();
        var handler = new UpdateCategoryCommandHandler(repo);

        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Updated Name");
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteCategoryCommand_ReturnsSuccess_WhenExists()
    {
        var repo = new FakeCategoryRepository();
        var existing = await repo.CreateAsync(new CategoryModel { Name = "To Delete" });
        var handler = new DeleteCategoryCommandHandler(repo);

        var result = await handler.Handle(new DeleteCategoryCommand(existing.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
}
