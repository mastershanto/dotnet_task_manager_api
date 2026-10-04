using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using InstructorClasses.Domain;

namespace InstructorClasses.Application.Features.InstructorClasses.Commands;

public record UpdateAttendanceCommand(int LessonId, int StudentId) : ICommand<UpdateAttendanceResult?>;
public record CreateClassCommand(CreateClassDto Dto) : ICommand<InstructorClassModel>;
public record DeleteClassCommand(int ClassId) : ICommand<bool>;
public record AddInstructorCommand(AddInstructorDto Dto) : ICommand<bool>;
public record AddStudentCommand(AddStudentDto Dto) : ICommand<bool>;

public class InstructorClassCommandHandlers :
    ICommandHandler<UpdateAttendanceCommand, UpdateAttendanceResult?>,
    ICommandHandler<CreateClassCommand, InstructorClassModel>,
    ICommandHandler<DeleteClassCommand, bool>,
    ICommandHandler<AddInstructorCommand, bool>,
    ICommandHandler<AddStudentCommand, bool>
{
    private readonly IInstructorClassRepository _repository;

    public InstructorClassCommandHandlers(IInstructorClassRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UpdateAttendanceResult?>> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
    {
        var res = await _repository.UpdateAttendanceAsync(request.LessonId, request.StudentId, cancellationToken);
        return res != null ? Result<UpdateAttendanceResult?>.Success(res) : Result<UpdateAttendanceResult?>.Failure("Class or student not found");
    }

    public async Task<Result<InstructorClassModel>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var created = await _repository.CreateClassAsync(request.Dto, cancellationToken);
        return Result<InstructorClassModel>.Success(created);
    }

    public async Task<Result<bool>> Handle(DeleteClassCommand request, CancellationToken cancellationToken)
    {
        var res = await _repository.DeleteClassAsync(request.ClassId, cancellationToken);
        return Result<bool>.Success(res);
    }

    public async Task<Result<bool>> Handle(AddInstructorCommand request, CancellationToken cancellationToken)
    {
        var res = await _repository.AddInstructorAsync(request.Dto, cancellationToken);
        return Result<bool>.Success(res);
    }

    public async Task<Result<bool>> Handle(AddStudentCommand request, CancellationToken cancellationToken)
    {
        var res = await _repository.AddStudentAsync(request.Dto, cancellationToken);
        return Result<bool>.Success(res);
    }
}
