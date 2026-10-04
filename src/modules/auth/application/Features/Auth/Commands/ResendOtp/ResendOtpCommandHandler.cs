using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.ResendOtp;

public class ResendOtpCommandHandler : ICommandHandler<ResendOtpCommand, RegisterResult>
{
    private readonly IAuthService _authService;

    public ResendOtpCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<RegisterResult>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResendOtpAsync(new PasswordResetRequest { Email = request.Email });
        return result.Status
            ? Result<RegisterResult>.Success(result)
            : Result<RegisterResult>.Failure(result.Message);
    }
}
