using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using StudentGroupClasses.Domain;

namespace StudentGroupClasses.Application.Features.StudentGroupClasses.Queries;

public record GetStudentGroupClassesQuery(string? Date) : IQuery<IEnumerable<GroupStudentClassModel>>;
public record GetStudentMyClassesQuery() : IQuery<IEnumerable<GroupStudentClassModel>>;
public record GetStudentClassHistoryQuery() : IQuery<IEnumerable<GroupStudentClassModel>>;
public record GetStudentClassDetailsQuery(int ClassId) : IQuery<GroupStudentClassModel?>;
public record GetStudentClassGuidesQuery() : IQuery<IEnumerable<ClassGuideModel>>;
public record GetStudentClassGuideDetailsQuery(int Id) : IQuery<ClassGuideModel?>;
public record GetStudentPurchaseProgramsQuery() : IQuery<IEnumerable<PurchaseProgramModel>>;
public record GetStudentMyCalendarQuery() : IQuery<IEnumerable<MyCalendarEvent>>;
public record SyncGoogleCalendarQuery() : IQuery<object>;

public class StudentGroupClassQueryHandlers :
    IQueryHandler<GetStudentGroupClassesQuery, IEnumerable<GroupStudentClassModel>>,
    IQueryHandler<GetStudentMyClassesQuery, IEnumerable<GroupStudentClassModel>>,
    IQueryHandler<GetStudentClassHistoryQuery, IEnumerable<GroupStudentClassModel>>,
    IQueryHandler<GetStudentClassDetailsQuery, GroupStudentClassModel?>,
    IQueryHandler<GetStudentClassGuidesQuery, IEnumerable<ClassGuideModel>>,
    IQueryHandler<GetStudentClassGuideDetailsQuery, ClassGuideModel?>,
    IQueryHandler<GetStudentPurchaseProgramsQuery, IEnumerable<PurchaseProgramModel>>,
    IQueryHandler<GetStudentMyCalendarQuery, IEnumerable<MyCalendarEvent>>,
    IQueryHandler<SyncGoogleCalendarQuery, object>
{
    private readonly IStudentGroupClassRepository _repo;

    public StudentGroupClassQueryHandlers(IStudentGroupClassRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IEnumerable<GroupStudentClassModel>>> Handle(GetStudentGroupClassesQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetGroupClassesAsync(request.Date, cancellationToken);
        return Result<IEnumerable<GroupStudentClassModel>>.Success(data);
    }

    public async Task<Result<IEnumerable<GroupStudentClassModel>>> Handle(GetStudentMyClassesQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetMyClassesAsync(cancellationToken);
        return Result<IEnumerable<GroupStudentClassModel>>.Success(data);
    }

    public async Task<Result<IEnumerable<GroupStudentClassModel>>> Handle(GetStudentClassHistoryQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetClassHistoryAsync(cancellationToken);
        return Result<IEnumerable<GroupStudentClassModel>>.Success(data);
    }

    public async Task<Result<GroupStudentClassModel?>> Handle(GetStudentClassDetailsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetClassDetailsAsync(request.ClassId, cancellationToken);
        return Result<GroupStudentClassModel?>.Success(data);
    }

    public async Task<Result<IEnumerable<ClassGuideModel>>> Handle(GetStudentClassGuidesQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetClassGuidesAsync(cancellationToken);
        return Result<IEnumerable<ClassGuideModel>>.Success(data);
    }

    public async Task<Result<ClassGuideModel?>> Handle(GetStudentClassGuideDetailsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetClassGuideDetailsAsync(request.Id, cancellationToken);
        return Result<ClassGuideModel?>.Success(data);
    }

    public async Task<Result<IEnumerable<PurchaseProgramModel>>> Handle(GetStudentPurchaseProgramsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetPurchaseProgramsAsync(cancellationToken);
        return Result<IEnumerable<PurchaseProgramModel>>.Success(data);
    }

    public async Task<Result<IEnumerable<MyCalendarEvent>>> Handle(GetStudentMyCalendarQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetMyCalendarEventsAsync(cancellationToken);
        return Result<IEnumerable<MyCalendarEvent>>.Success(data);
    }

    public async Task<Result<object>> Handle(SyncGoogleCalendarQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.SyncGoogleCalendarAsync(cancellationToken);
        return Result<object>.Success(data);
    }
}
