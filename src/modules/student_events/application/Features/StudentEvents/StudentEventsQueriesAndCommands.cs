using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using StudentEvents.Domain;

namespace StudentEvents.Application.Features.StudentEvents;

public record GetStudentEventsQuery(string? Type) : IQuery<IEnumerable<StudentEventModel>>;
public record GetStudentEventDetailsQuery(int Id) : IQuery<StudentEventModel?>;
public record GetEventParticipantsQuery(int EventId) : IQuery<IEnumerable<EventParticipantModel>>;
public record BookStudentEventCommand(BookEventRequest Request) : ICommand<bool>;
public record GetDancerOfTheMonthQuery() : IQuery<DancerOfTheMonthModel?>;
public record SearchStudentsQuery(string? Query) : IQuery<IEnumerable<SearchStudentModel>>;

public class StudentEventsHandlers :
    IQueryHandler<GetStudentEventsQuery, IEnumerable<StudentEventModel>>,
    IQueryHandler<GetStudentEventDetailsQuery, StudentEventModel?>,
    IQueryHandler<GetEventParticipantsQuery, IEnumerable<EventParticipantModel>>,
    ICommandHandler<BookStudentEventCommand, bool>,
    IQueryHandler<GetDancerOfTheMonthQuery, DancerOfTheMonthModel?>,
    IQueryHandler<SearchStudentsQuery, IEnumerable<SearchStudentModel>>
{
    private readonly IStudentEventsRepository _repo;

    public StudentEventsHandlers(IStudentEventsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IEnumerable<StudentEventModel>>> Handle(GetStudentEventsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetEventsAsync(request.Type, cancellationToken);
        return Result<IEnumerable<StudentEventModel>>.Success(data);
    }

    public async Task<Result<StudentEventModel?>> Handle(GetStudentEventDetailsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetEventDetailsAsync(request.Id, cancellationToken);
        return Result<StudentEventModel?>.Success(data);
    }

    public async Task<Result<IEnumerable<EventParticipantModel>>> Handle(GetEventParticipantsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetParticipantsAsync(request.EventId, cancellationToken);
        return Result<IEnumerable<EventParticipantModel>>.Success(data);
    }

    public async Task<Result<bool>> Handle(BookStudentEventCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.BookEventAsync(request.Request, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<DancerOfTheMonthModel?>> Handle(GetDancerOfTheMonthQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetDancerOfTheMonthAsync(cancellationToken);
        return Result<DancerOfTheMonthModel?>.Success(data);
    }

    public async Task<Result<IEnumerable<SearchStudentModel>>> Handle(SearchStudentsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.SearchStudentsAsync(request.Query, cancellationToken);
        return Result<IEnumerable<SearchStudentModel>>.Success(data);
    }
}
