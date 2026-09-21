using BuildingBlocks.Abstractions;
using Projects.Application.Features.Projects.Commands.CreateProject;
using Projects.Application.Features.Projects.Commands.DeleteProject;
using Projects.Application.Features.Projects.Commands.UpdateProject;
using Projects.Application.Features.Projects.Queries.GetProjectById;
using Projects.Application.Features.Projects.Queries.GetProjects;
using Projects.Domain;

namespace Api.Tests;

public class FakeProjectRepository : IProjectRepository
{
    private readonly List<ProjectModel> _projects = new();

    public Task<IEnumerable<ProjectModel>> ListAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<ProjectModel>>(_projects.ToList());

    public Task<ProjectModel?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_projects.FirstOrDefault(p => p.Id == id));

    public Task<ProjectModel> CreateAsync(ProjectModel project, CancellationToken cancellationToken = default)
    {
        _projects.Add(project);
        return Task.FromResult(project);
    }

    public Task<ProjectModel?> UpdateAsync(Guid id, ProjectModel project, CancellationToken cancellationToken = default)
    {
        var idx = _projects.FindIndex(p => p.Id == id);
        if (idx == -1) return Task.FromResult<ProjectModel?>(null);
        _projects[idx] = project;
        return Task.FromResult<ProjectModel?>(project);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = _projects.FirstOrDefault(p => p.Id == id);
        if (item is null) return Task.FromResult(false);
        _projects.Remove(item);
        return Task.FromResult(true);
    }
}

public class ProjectCqrsTests
{
    [Fact]
    public async Task CreateProjectCommand_Success_WhenValid()
    {
        var repo = new FakeProjectRepository();
        var handler = new CreateProjectCommandHandler(repo);

        var command = new CreateProjectCommand("Alpha Project", "Top secret enterprise project", ProjectStatus.Active);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Alpha Project", result.Value.Name);
        Assert.Equal(ProjectStatus.Active, result.Value.Status);
    }

    [Fact]
    public void CreateProjectValidator_Fails_WhenNameIsTooShort()
    {
        var validator = new CreateProjectCommandValidator();
        var command = new CreateProjectCommand("A", "Short name");

        var validationResult = validator.Validate(command);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(CreateProjectCommand.Name));
    }

    [Fact]
    public void CreateProjectValidator_Fails_WhenEndDateBeforeStartDate()
    {
        var validator = new CreateProjectCommandValidator();
        var command = new CreateProjectCommand(
            "Valid Project",
            "Description",
            StartDate: DateTime.UtcNow.AddDays(5),
            EndDate: DateTime.UtcNow.AddDays(2)
        );

        var validationResult = validator.Validate(command);

        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public async Task GetProjectsQuery_FiltersByStatus()
    {
        var repo = new FakeProjectRepository();
        await repo.CreateAsync(new ProjectModel { Name = "Active P", Status = ProjectStatus.Active });
        await repo.CreateAsync(new ProjectModel { Name = "Archived P", Status = ProjectStatus.Archived });

        var handler = new GetProjectsQueryHandler(repo);
        var result = await handler.Handle(new GetProjectsQuery(Status: ProjectStatus.Active), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.Equal("Active P", result.Value!.First().Name);
    }

    [Fact]
    public async Task UpdateProjectCommand_ReturnsFailure_WhenNotFound()
    {
        var repo = new FakeProjectRepository();
        var handler = new UpdateProjectCommandHandler(repo);

        var command = new UpdateProjectCommand(Guid.NewGuid(), "Updated Name", "Desc", ProjectStatus.Completed, "#000", null, null, null);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteProjectCommand_ReturnsSuccess_WhenExists()
    {
        var repo = new FakeProjectRepository();
        var existing = await repo.CreateAsync(new ProjectModel { Name = "To be deleted" });
        var handler = new DeleteProjectCommandHandler(repo);

        var result = await handler.Handle(new DeleteProjectCommand(existing.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
}
