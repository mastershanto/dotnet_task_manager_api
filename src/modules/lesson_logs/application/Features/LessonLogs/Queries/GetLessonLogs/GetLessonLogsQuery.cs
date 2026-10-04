using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using LessonLogs.Domain;

namespace LessonLogs.Application.Features.LessonLogs.Queries.GetLessonLogs;

public record GetLessonLogsQuery(int? UserId, string? Date) : IQuery<LessonLogsResponseData>;

public class GetLessonLogsQueryHandler : IQueryHandler<GetLessonLogsQuery, LessonLogsResponseData>
{
    private readonly ILessonLogsRepository _repository;

    public GetLessonLogsQueryHandler(ILessonLogsRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<LessonLogsResponseData>> Handle(GetLessonLogsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repository.GetLogsAsync(request.UserId, request.Date, cancellationToken);
        return Result<LessonLogsResponseData>.Success(data);
    }
}
