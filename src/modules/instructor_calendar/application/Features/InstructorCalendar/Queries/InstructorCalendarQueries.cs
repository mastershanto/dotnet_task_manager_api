using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using InstructorCalendar.Domain;

namespace InstructorCalendar.Application.Features.InstructorCalendar.Queries;

public record GetCalendarEventsQuery(int? InstructorId, int? StudioId, string? Date) : IQuery<IEnumerable<CalendarEventModel>>;
public record GetCalendarInstructorsQuery() : IQuery<IEnumerable<CalendarInstructorItem>>;
public record GetCalendarStudiosQuery() : IQuery<IEnumerable<CalendarStudioItem>>;

public class InstructorCalendarQueryHandlers :
    IQueryHandler<GetCalendarEventsQuery, IEnumerable<CalendarEventModel>>,
    IQueryHandler<GetCalendarInstructorsQuery, IEnumerable<CalendarInstructorItem>>,
    IQueryHandler<GetCalendarStudiosQuery, IEnumerable<CalendarStudioItem>>
{
    private readonly IInstructorCalendarRepository _repository;

    public InstructorCalendarQueryHandlers(IInstructorCalendarRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<CalendarEventModel>>> Handle(GetCalendarEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _repository.GetCalendarEventsAsync(request.InstructorId, request.StudioId, request.Date, cancellationToken);
        return Result<IEnumerable<CalendarEventModel>>.Success(events);
    }

    public async Task<Result<IEnumerable<CalendarInstructorItem>>> Handle(GetCalendarInstructorsQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetCalendarInstructorsAsync(cancellationToken);
        return Result<IEnumerable<CalendarInstructorItem>>.Success(list);
    }

    public async Task<Result<IEnumerable<CalendarStudioItem>>> Handle(GetCalendarStudiosQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetCalendarStudiosAsync(cancellationToken);
        return Result<IEnumerable<CalendarStudioItem>>.Success(list);
    }
}
