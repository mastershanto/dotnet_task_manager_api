using BuildingBlocks.CQRS;
using Projects.Domain;

namespace Projects.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// নতুন প্রজেক্ট তৈরির CQRS Command:
/// </summary>
public record CreateProjectCommand(
    string Name,
    string Description,
    ProjectStatus Status = ProjectStatus.Active,
    string Color = "#3B82F6",
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    Guid? OwnerId = null
) : ICommand<ProjectModel>;
