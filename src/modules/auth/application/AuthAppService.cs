using Auth.Domain;
using BuildingBlocks.Abstractions;

namespace Auth.Application;

public class AuthAppService : IAuthAppService
{
    private readonly IAuthService _authService;

    public AuthAppService(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResult>> LoginAsync(AuthenticationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<AuthResult>.Failure("Email and password are required");

        var result = await _authService.AuthenticateAsync(request);
        return result.Success ? Result<AuthResult>.Success(result) : Result<AuthResult>.Failure(result.Message);
    }
}
