using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using InstructorSyllabus.Domain;

namespace InstructorSyllabus.Application.Features.InstructorSyllabus.Queries;

public record GetInstructorSyllabiQuery() : IQuery<IEnumerable<SyllabusSummaryModel>>;
public record GetStudentSyllabusQuery(int StudentId, int SyllabusId) : IQuery<object>;

public class InstructorSyllabusQueryHandlers :
    IQueryHandler<GetInstructorSyllabiQuery, IEnumerable<SyllabusSummaryModel>>,
    IQueryHandler<GetStudentSyllabusQuery, object>
{
    private readonly IInstructorSyllabusRepository _repository;

    public InstructorSyllabusQueryHandlers(IInstructorSyllabusRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<SyllabusSummaryModel>>> Handle(GetInstructorSyllabiQuery request, CancellationToken cancellationToken)
    {
        var syllabi = await _repository.GetSyllabiAsync(cancellationToken);
        return Result<IEnumerable<SyllabusSummaryModel>>.Success(syllabi);
    }

    public async Task<Result<object>> Handle(GetStudentSyllabusQuery request, CancellationToken cancellationToken)
    {
        var data = await _repository.GetStudentSyllabusAsync(request.StudentId, request.SyllabusId, cancellationToken);
        return Result<object>.Success(data);
    }
}
