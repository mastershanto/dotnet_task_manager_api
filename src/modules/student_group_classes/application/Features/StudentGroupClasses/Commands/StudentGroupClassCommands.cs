using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using StudentGroupClasses.Domain;

namespace StudentGroupClasses.Application.Features.StudentGroupClasses.Commands;

public record EnrollGroupClassCommand(int ClassId) : ICommand<bool>;
public record CancelGroupClassCommand(int ClassId) : ICommand<bool>;
public record CancelRecurringGroupClassCommand(int ClassId) : ICommand<bool>;
public record PurchaseProgramCommand(int ProgramId) : ICommand<bool>;

public class StudentGroupClassCommandHandlers :
    ICommandHandler<EnrollGroupClassCommand, bool>,
    ICommandHandler<CancelGroupClassCommand, bool>,
    ICommandHandler<CancelRecurringGroupClassCommand, bool>,
    ICommandHandler<PurchaseProgramCommand, bool>
{
    private readonly IStudentGroupClassRepository _repo;

    public StudentGroupClassCommandHandlers(IStudentGroupClassRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<bool>> Handle(EnrollGroupClassCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.EnrollClassAsync(request.ClassId, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(CancelGroupClassCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.CancelClassAsync(request.ClassId, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(CancelRecurringGroupClassCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.CancelRecurringClassAsync(request.ClassId, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(PurchaseProgramCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.PurchaseProgramAsync(request.ProgramId, cancellationToken);
        return Result<bool>.Success(ok);
    }
}
