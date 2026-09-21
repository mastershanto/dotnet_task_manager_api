using BuildingBlocks.CQRS;
using Projects.Domain;

namespace Projects.Application.Features.Projects.Queries.GetProjectById;

/// <summary>
/// নির্দিষ্ট আইডির প্রজেক্ট রিড করার CQRS Query:
/// </summary>
public record GetProjectByIdQuery(Guid Id) : IQuery<ProjectModel>;
