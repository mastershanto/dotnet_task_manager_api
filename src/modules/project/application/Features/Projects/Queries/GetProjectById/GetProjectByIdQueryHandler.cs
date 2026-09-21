using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Projects.Domain;

namespace Projects.Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler : IQueryHandler<GetProjectByIdQuery, ProjectModel>
{
    private readonly IProjectRepository _repository;

    public GetProjectByIdQueryHandler(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProjectModel>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _repository.GetAsync(request.Id, cancellationToken);
        return project is null
            ? Result<ProjectModel>.Failure($"Project with ID '{request.Id}' was not found.")
            : Result<ProjectModel>.Success(project);
    }
}
