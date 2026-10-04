using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Abstractions.Results;
using StudentChildren.Domain;

namespace StudentChildren.Application.Features.StudentChildren;

public record GetChildrenQuery() : IQuery<IEnumerable<StudentKidModel>>;
public record GetChildByIdQuery(int ChildId) : IQuery<StudentKidModel?>;
public record CreateChildCommand(CreateStudentKidDto Dto) : ICommand<StudentKidModel>;
public record UpdateChildCommand(int ChildId, UpdateStudentKidDto Dto) : ICommand<StudentKidModel?>;
public record DeleteChildCommand(int ChildId) : ICommand<bool>;

public class StudentChildrenHandlers :
    IQueryHandler<GetChildrenQuery, IEnumerable<StudentKidModel>>,
    IQueryHandler<GetChildByIdQuery, StudentKidModel?>,
    ICommandHandler<CreateChildCommand, StudentKidModel>,
    ICommandHandler<UpdateChildCommand, StudentKidModel?>,
    ICommandHandler<DeleteChildCommand, bool>
{
    private readonly IStudentChildrenRepository _repo;

    public StudentChildrenHandlers(IStudentChildrenRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IEnumerable<StudentKidModel>>> Handle(GetChildrenQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetChildrenAsync(cancellationToken);
        return Result<IEnumerable<StudentKidModel>>.Success(data);
    }

    public async Task<Result<StudentKidModel?>> Handle(GetChildByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetChildByIdAsync(request.ChildId, cancellationToken);
        return Result<StudentKidModel?>.Success(data);
    }

    public async Task<Result<StudentKidModel>> Handle(CreateChildCommand request, CancellationToken cancellationToken)
    {
        var data = await _repo.CreateChildAsync(request.Dto, cancellationToken);
        return Result<StudentKidModel>.Success(data);
    }

    public async Task<Result<StudentKidModel?>> Handle(UpdateChildCommand request, CancellationToken cancellationToken)
    {
        var data = await _repo.UpdateChildAsync(request.ChildId, request.Dto, cancellationToken);
        return Result<StudentKidModel?>.Success(data);
    }

    public async Task<Result<bool>> Handle(DeleteChildCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.DeleteChildAsync(request.ChildId, cancellationToken);
        return Result<bool>.Success(ok);
    }
}
