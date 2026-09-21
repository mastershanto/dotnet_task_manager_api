using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Projects.Domain;

namespace Projects.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : ICommandHandler<UpdateProjectCommand, ProjectModel>
{
    private readonly IProjectRepository _repository;

    public UpdateProjectCommandHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProjectModel>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<ProjectModel>.Failure($"Project with ID '{request.Id}' was not found.");
        }

        var projectToUpdate = existing with
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Status = request.Status,
            Color = string.IsNullOrWhiteSpace(request.Color) ? existing.Color : request.Color.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            OwnerId = request.OwnerId,
            UpdatedAt = DateTime.UtcNow
        };

        var updated = await _repository.UpdateAsync(request.Id, projectToUpdate, cancellationToken);
        return updated is null
            ? Result<ProjectModel>.Failure("Failed to update project.")
            : Result<ProjectModel>.Success(updated);
    }
}
