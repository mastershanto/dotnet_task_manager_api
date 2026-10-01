using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.DeleteAccount;

/// <summary>
/// একাউন্ট ডিলিট হ্যান্ডলার:
/// ১. পাসওয়ার্ড নিশ্চিতকরণ যাচাই করে।
/// ২. সঠিক হলে ইউজার রেকর্ড মুছে ফেলে।
/// </summary>
public class DeleteAccountCommandHandler : ICommandHandler<DeleteAccountCommand, bool>
{
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasherService _hasher;

    public DeleteAccountCommandHandler(
        IAuthRepository authRepository,
        IPasswordHasherService hasher)
    {
        _authRepository = authRepository;
        _hasher = hasher;
    }

    public async Task<Result<bool>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<bool>.Failure("User account not found.");
        }

        var isPasswordValid = _hasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<bool>.Failure("Invalid password. Account deletion aborted.");
        }

        var deleted = await _authRepository.DeleteUserAsync(request.UserId, cancellationToken);
        return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Failed to delete user account.");
    }
}
