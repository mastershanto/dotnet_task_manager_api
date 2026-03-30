using App.Features.Auth.Domain;

namespace App.Features.Auth.Data;

public class AuthService : IAuthService
{
    private const string DemoEmail = "admin@example.com";
    private const string DemoPassword = "Password123";

    public Task<AuthResult> AuthenticateAsync(AuthenticationRequest request)
    {
        if (request.Email.Equals(DemoEmail, StringComparison.OrdinalIgnoreCase) && request.Password == DemoPassword)
        {
            return Task.FromResult(new AuthResult(true, "Logged in", "demo-jwt-token"));
        }

        return Task.FromResult(new AuthResult(false, "Invalid credentials", null));
    }
}
