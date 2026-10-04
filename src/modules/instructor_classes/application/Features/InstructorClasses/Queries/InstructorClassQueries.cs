using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using InstructorClasses.Domain;

namespace InstructorClasses.Application.Features.InstructorClasses.Queries;

public record GetInstructorClassesQuery(string? Date) : IQuery<IEnumerable<InstructorClassModel>>;
public record GetInstructorClassByIdQuery(int LessonId) : IQuery<InstructorClassModel?>;
public record GetClassStudentDetailsQuery(int LessonId, int StudentId) : IQuery<object?>;
public record GetRepeatPatternsQuery() : IQuery<IEnumerable<RepeatPatternModel>>;
public record SearchStudentsQuery(string? Q) : IQuery<IEnumerable<ClassUserSummary>>;
public record SearchInstructorsQuery(string? Q) : IQuery<IEnumerable<ClassUserSummary>>;
public record SearchStudiosQuery(string? Q) : IQuery<IEnumerable<StudioSummary>>;

public class InstructorClassQueryHandlers :
    IQueryHandler<GetInstructorClassesQuery, IEnumerable<InstructorClassModel>>,
    IQueryHandler<GetInstructorClassByIdQuery, InstructorClassModel?>,
    IQueryHandler<GetClassStudentDetailsQuery, object?>,
    IQueryHandler<GetRepeatPatternsQuery, IEnumerable<RepeatPatternModel>>,
    IQueryHandler<SearchStudentsQuery, IEnumerable<ClassUserSummary>>,
    IQueryHandler<SearchInstructorsQuery, IEnumerable<ClassUserSummary>>,
    IQueryHandler<SearchStudiosQuery, IEnumerable<StudioSummary>>
{
    private readonly IInstructorClassRepository _repository;

    public InstructorClassQueryHandlers(IInstructorClassRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<InstructorClassModel>>> Handle(GetInstructorClassesQuery request, CancellationToken cancellationToken)
    {
        var classes = await _repository.GetClassesAsync(request.Date, cancellationToken);
        return Result<IEnumerable<InstructorClassModel>>.Success(classes);
    }

    public async Task<Result<InstructorClassModel?>> Handle(GetInstructorClassByIdQuery request, CancellationToken cancellationToken)
    {
        var cls = await _repository.GetClassByIdAsync(request.LessonId, cancellationToken);
        return Result<InstructorClassModel?>.Success(cls);
    }

    public async Task<Result<object?>> Handle(GetClassStudentDetailsQuery request, CancellationToken cancellationToken)
    {
        var details = await _repository.GetClassStudentDetailsAsync(request.LessonId, request.StudentId, cancellationToken);
        return Result<object?>.Success(details);
    }

    public async Task<Result<IEnumerable<RepeatPatternModel>>> Handle(GetRepeatPatternsQuery request, CancellationToken cancellationToken)
    {
        var patterns = await _repository.GetRepeatPatternsAsync(cancellationToken);
        return Result<IEnumerable<RepeatPatternModel>>.Success(patterns);
    }

    public async Task<Result<IEnumerable<ClassUserSummary>>> Handle(SearchStudentsQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.SearchStudentsAsync(request.Q, cancellationToken);
        return Result<IEnumerable<ClassUserSummary>>.Success(list);
    }

    public async Task<Result<IEnumerable<ClassUserSummary>>> Handle(SearchInstructorsQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.SearchInstructorsAsync(request.Q, cancellationToken);
        return Result<IEnumerable<ClassUserSummary>>.Success(list);
    }

    public async Task<Result<IEnumerable<StudioSummary>>> Handle(SearchStudiosQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.SearchStudiosAsync(request.Q, cancellationToken);
        return Result<IEnumerable<StudioSummary>>.Success(list);
    }
}
