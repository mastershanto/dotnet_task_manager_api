using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Projects.Domain;

namespace Projects.Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler : IQueryHandler<GetProjectsQuery, IEnumerable<ProjectModel>>
{
    private readonly IProjectRepository _repository;

    public GetProjectsQueryHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<ProjectModel>>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _repository.ListAsync(cancellationToken);

        if (request.Status.HasValue)
        {
            projects = projects.Where(p => p.Status == request.Status.Value);
        }

        return Result<IEnumerable<ProjectModel>>.Success(projects);
    }
}
