using BuildingBlocks.CQRS;
using Projects.Domain;

namespace Projects.Application.Features.Projects.Commands.UpdateProject;

/// <summary>
/// বিদ্যমান প্রজেক্টের তথ্য আপডেট করার CQRS Command:
/// </summary>
public record UpdateProjectCommand(
    Guid Id,
    string Name,
    string Description,
    ProjectStatus Status,
    string Color,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid? OwnerId
) : ICommand<ProjectModel>;
