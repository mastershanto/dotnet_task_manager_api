using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResult>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var authReq = new AuthenticationRequest
        {
            Email = request.Email.Trim(),
            Password = request.Password
        };

        var result = await _authService.AuthenticateAsync(authReq);
        return result.Success 
            ? Result<AuthResult>.Success(result) 
            : Result<AuthResult>.Failure(result.Message);
    }
}
