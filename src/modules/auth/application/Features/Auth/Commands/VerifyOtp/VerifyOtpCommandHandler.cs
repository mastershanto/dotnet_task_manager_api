using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.VerifyOtp;

public class VerifyOtpCommandHandler : ICommandHandler<VerifyOtpCommand, OtpResult>
{
    private readonly IAuthService _authService;

    public VerifyOtpCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<OtpResult>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.VerifyOtpAsync(new OtpRequest { Email = request.Email, Otp = request.Otp });
        return result.Status
            ? Result<OtpResult>.Success(result)
            : Result<OtpResult>.Failure(result.Message);
    }
}
