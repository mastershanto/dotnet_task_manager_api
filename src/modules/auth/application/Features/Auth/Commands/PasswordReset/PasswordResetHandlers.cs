using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.PasswordReset;

public class PasswordResetRequestHandler : ICommandHandler<PasswordResetRequestCommand, PasswordResetResult>
{
    private readonly IAuthService _authService;
    public PasswordResetRequestHandler(IAuthService authService) => _authService = authService;

    public async Task<Result<PasswordResetResult>> Handle(PasswordResetRequestCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.PasswordResetRequestAsync(new PasswordResetRequest { Email = request.Email });
        return result.Status ? Result<PasswordResetResult>.Success(result) : Result<PasswordResetResult>.Failure(result.Message);
    }
}

public class PasswordResetResendHandler : ICommandHandler<PasswordResetResendCommand, PasswordResetResult>
{
    private readonly IAuthService _authService;
    public PasswordResetResendHandler(IAuthService authService) => _authService = authService;

    public async Task<Result<PasswordResetResult>> Handle(PasswordResetResendCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.PasswordResetResendAsync(new PasswordResetRequest { Email = request.Email });
        return result.Status ? Result<PasswordResetResult>.Success(result) : Result<PasswordResetResult>.Failure(result.Message);
    }
}

public class PasswordResetVerifyHandler : ICommandHandler<PasswordResetVerifyCommand, PasswordResetResult>
{
    private readonly IAuthService _authService;
    public PasswordResetVerifyHandler(IAuthService authService) => _authService = authService;

    public async Task<Result<PasswordResetResult>> Handle(PasswordResetVerifyCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.PasswordResetVerifyAsync(new PasswordResetVerifyRequest { Email = request.Email, Otp = request.Otp });
        return result.Status ? Result<PasswordResetResult>.Success(result) : Result<PasswordResetResult>.Failure(result.Message);
    }
}

public class PasswordResetConfirmHandler : ICommandHandler<PasswordResetConfirmCommand, PasswordResetResult>
{
    private readonly IAuthService _authService;
    public PasswordResetConfirmHandler(IAuthService authService) => _authService = authService;

    public async Task<Result<PasswordResetResult>> Handle(PasswordResetConfirmCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.PasswordResetConfirmAsync(new PasswordResetConfirmRequest { ResetToken = request.ResetToken, Password = request.Password });
        return result.Status ? Result<PasswordResetResult>.Success(result) : Result<PasswordResetResult>.Failure(result.Message);
    }
}
