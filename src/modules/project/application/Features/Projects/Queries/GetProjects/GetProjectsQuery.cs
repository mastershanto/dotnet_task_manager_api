using BuildingBlocks.CQRS;
using Projects.Domain;

namespace Projects.Application.Features.Projects.Queries.GetProjects;

/// <summary>
/// সমস্ত প্রজেক্ট পাওয়ার CQRS Query:
/// </summary>
public record GetProjectsQuery(
    ProjectStatus? Status = null
) : IQuery<IEnumerable<ProjectModel>>;
