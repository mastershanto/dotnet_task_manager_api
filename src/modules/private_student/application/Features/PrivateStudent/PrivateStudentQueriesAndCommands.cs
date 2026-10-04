using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Abstractions.Results;
using PrivateStudent.Domain;

namespace PrivateStudent.Application.Features.PrivateStudent;

// Queries
public record GetPrivateClassesQuery() : IQuery<IEnumerable<PrivateClassModel>>;
public record GetNextPrivateClassQuery() : IQuery<PrivateClassModel?>;
public record GetAnnouncementsQuery() : IQuery<IEnumerable<AnnouncementModel>>;
public record GetPrivatePurchaseProgramsQuery() : IQuery<IEnumerable<PrivatePackageModel>>;
public record GetMyPackagesQuery() : IQuery<IEnumerable<PrivatePackageModel>>;
public record GetMyActivePackageQuery() : IQuery<PrivatePackageModel?>;
public record GetStudentPartnerQuery() : IQuery<StudentPartnerModel>;

// Commands
public record ReadAnnouncementCommand(int Id) : ICommand<bool>;
public record PurchasePrivateProgramCommand(int PackageId) : ICommand<bool>;
public record RequestPartnerCommand(int UserId) : ICommand<bool>;
public record CancelPartnerRequestCommand(int UserId) : ICommand<bool>;
public record AcceptPartnerCommand(int UserId) : ICommand<bool>;
public record DeclinePartnerCommand(int UserId) : ICommand<bool>;
public record RemovePartnerCommand() : ICommand<bool>;

public class PrivateStudentHandlers :
    IQueryHandler<GetPrivateClassesQuery, IEnumerable<PrivateClassModel>>,
    IQueryHandler<GetNextPrivateClassQuery, PrivateClassModel?>,
    IQueryHandler<GetAnnouncementsQuery, IEnumerable<AnnouncementModel>>,
    IQueryHandler<GetPrivatePurchaseProgramsQuery, IEnumerable<PrivatePackageModel>>,
    IQueryHandler<GetMyPackagesQuery, IEnumerable<PrivatePackageModel>>,
    IQueryHandler<GetMyActivePackageQuery, PrivatePackageModel?>,
    IQueryHandler<GetStudentPartnerQuery, StudentPartnerModel>,
    ICommandHandler<ReadAnnouncementCommand, bool>,
    ICommandHandler<PurchasePrivateProgramCommand, bool>,
    ICommandHandler<RequestPartnerCommand, bool>,
    ICommandHandler<CancelPartnerRequestCommand, bool>,
    ICommandHandler<AcceptPartnerCommand, bool>,
    ICommandHandler<DeclinePartnerCommand, bool>,
    ICommandHandler<RemovePartnerCommand, bool>
{
    private readonly IPrivateStudentRepository _repo;

    public PrivateStudentHandlers(IPrivateStudentRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IEnumerable<PrivateClassModel>>> Handle(GetPrivateClassesQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetClassesAsync(cancellationToken);
        return Result<IEnumerable<PrivateClassModel>>.Success(data);
    }

    public async Task<Result<PrivateClassModel?>> Handle(GetNextPrivateClassQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetNextPrivateClassAsync(cancellationToken);
        return Result<PrivateClassModel?>.Success(data);
    }

    public async Task<Result<IEnumerable<AnnouncementModel>>> Handle(GetAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetAnnouncementsAsync(cancellationToken);
        return Result<IEnumerable<AnnouncementModel>>.Success(data);
    }

    public async Task<Result<IEnumerable<PrivatePackageModel>>> Handle(GetPrivatePurchaseProgramsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetPurchaseProgramsAsync(cancellationToken);
        return Result<IEnumerable<PrivatePackageModel>>.Success(data);
    }

    public async Task<Result<IEnumerable<PrivatePackageModel>>> Handle(GetMyPackagesQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetMyPackagesAsync(cancellationToken);
        return Result<IEnumerable<PrivatePackageModel>>.Success(data);
    }

    public async Task<Result<PrivatePackageModel?>> Handle(GetMyActivePackageQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetMyActivePackageAsync(cancellationToken);
        return Result<PrivatePackageModel?>.Success(data);
    }

    public async Task<Result<StudentPartnerModel>> Handle(GetStudentPartnerQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetPartnerInfoAsync(cancellationToken);
        return Result<StudentPartnerModel>.Success(data);
    }

    public async Task<Result<bool>> Handle(ReadAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.MarkAnnouncementReadAsync(request.Id, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(PurchasePrivateProgramCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.PurchaseProgramAsync(request.PackageId, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(RequestPartnerCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.RequestPartnerAsync(request.UserId, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(CancelPartnerRequestCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.CancelPartnerRequestAsync(request.UserId, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(AcceptPartnerCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.AcceptPartnerAsync(request.UserId, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(DeclinePartnerCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.DeclinePartnerAsync(request.UserId, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(RemovePartnerCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.RemovePartnerAsync(cancellationToken);
        return Result<bool>.Success(ok);
    }
}
