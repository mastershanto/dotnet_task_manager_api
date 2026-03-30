using App.Features.Auth.Data;
using App.Features.Auth.Domain;

namespace App.Features.Auth.Presentation;

public static class AuthEndpoints
{
    public static void MapAuth(this WebApplication app)
    {
        app.MapPost("/auth/login", async (IAuthService authService, AuthenticationRequest request) =>
        {
            var result = await authService.AuthenticateAsync(request);
            return result.Success ? Results.Ok(result) : Results.Unauthorized();
        });
    }
}
