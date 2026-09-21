using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Projects.Domain;

namespace Projects.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand, ProjectModel>
{
    private readonly IProjectRepository _repository;

    public CreateProjectCommandHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProjectModel>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Status = request.Status,
            Color = string.IsNullOrWhiteSpace(request.Color) ? "#3B82F6" : request.Color.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            OwnerId = request.OwnerId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(project, cancellationToken);
        return Result<ProjectModel>.Success(created);
    }
}
