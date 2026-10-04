using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterResult>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<RegisterResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registerReq = new RegisterRequest
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Password = request.Password,
            Type = request.Type,
            AgreeToTerms = request.AgreeToTerms,
            IsFullProgram = request.IsFullProgram
        };

        var result = await _authService.RegisterAsync(registerReq);
        return result.Status
            ? Result<RegisterResult>.Success(result)
            : Result<RegisterResult>.Failure(result.Message);
    }
}
