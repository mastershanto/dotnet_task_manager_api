using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Profile.Domain;

namespace Profile.Application.Features.Profiles.Commands;

// Commands
public record UpdateProfileCommand(ProfileUpdateDto Dto) : ICommand<ProfileModel>;
public record ToggleProfileVisibilityCommand() : ICommand<bool>;
public record UpdatePasswordCommand(UpdatePasswordDto Dto) : ICommand<bool>;
public record DeleteProfileCommand() : ICommand<bool>;
public record StoreCardCommand(StoreCardDto Dto) : ICommand<UserCardModel>;
public record DeleteCardCommand(int Id) : ICommand<bool>;
public record SwitchChildCommand(int ChildId) : ICommand<ProfileModel?>;
public record SwitchToParentCommand() : ICommand<ProfileModel?>;

// Handlers
public class ProfileCommandHandlers :
    ICommandHandler<UpdateProfileCommand, ProfileModel>,
    ICommandHandler<ToggleProfileVisibilityCommand, bool>,
    ICommandHandler<UpdatePasswordCommand, bool>,
    ICommandHandler<DeleteProfileCommand, bool>,
    ICommandHandler<StoreCardCommand, UserCardModel>,
    ICommandHandler<DeleteCardCommand, bool>,
    ICommandHandler<SwitchChildCommand, ProfileModel?>,
    ICommandHandler<SwitchToParentCommand, ProfileModel?>
{
    private readonly IProfileRepository _repository;

    public ProfileCommandHandlers(IProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProfileModel>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var updated = await _repository.UpdateProfileAsync(request.Dto, cancellationToken);
        return Result<ProfileModel>.Success(updated);
    }

    public async Task<Result<bool>> Handle(ToggleProfileVisibilityCommand request, CancellationToken cancellationToken)
    {
        var res = await _repository.ToggleVisibilityAsync(cancellationToken);
        return Result<bool>.Success(res);
    }

    public async Task<Result<bool>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var res = await _repository.UpdatePasswordAsync(request.Dto, cancellationToken);
        return res ? Result<bool>.Success(true) : Result<bool>.Failure("Current password is incorrect");
    }

    public async Task<Result<bool>> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        var res = await _repository.DeleteAccountAsync(cancellationToken);
        return Result<bool>.Success(res);
    }

    public async Task<Result<UserCardModel>> Handle(StoreCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _repository.StoreCardAsync(request.Dto, cancellationToken);
        return Result<UserCardModel>.Success(card);
    }

    public async Task<Result<bool>> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
    {
        var res = await _repository.DeleteCardAsync(request.Id, cancellationToken);
        return Result<bool>.Success(res);
    }

    public async Task<Result<ProfileModel?>> Handle(SwitchChildCommand request, CancellationToken cancellationToken)
    {
        var p = await _repository.SwitchChildAsync(request.ChildId, cancellationToken);
        return Result<ProfileModel?>.Success(p);
    }

    public async Task<Result<ProfileModel?>> Handle(SwitchToParentCommand request, CancellationToken cancellationToken)
    {
        var p = await _repository.SwitchToParentAsync(cancellationToken);
        return Result<ProfileModel?>.Success(p);
    }
}
