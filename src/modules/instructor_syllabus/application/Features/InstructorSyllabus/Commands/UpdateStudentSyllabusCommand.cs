using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using InstructorSyllabus.Domain;

namespace InstructorSyllabus.Application.Features.InstructorSyllabus.Commands;

public record UpdateStudentSyllabusCommand(UpdateStudentSyllabusDto Dto) : ICommand<StudentSyllabusItemModel>;

public class UpdateStudentSyllabusCommandHandler : ICommandHandler<UpdateStudentSyllabusCommand, StudentSyllabusItemModel>
{
    private readonly IInstructorSyllabusRepository _repository;

    public UpdateStudentSyllabusCommandHandler(IInstructorSyllabusRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<StudentSyllabusItemModel>> Handle(UpdateStudentSyllabusCommand request, CancellationToken cancellationToken)
    {
        var updated = await _repository.UpdateStudentSyllabusAsync(request.Dto, cancellationToken);
        return Result<StudentSyllabusItemModel>.Success(updated);
    }
}
