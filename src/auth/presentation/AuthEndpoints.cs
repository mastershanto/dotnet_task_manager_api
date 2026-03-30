using App.Features.Auth.Application;
using App.Features.Auth.Domain;

namespace App.Features.Auth.Presentation;

public static class AuthEndpoints
{
    public static void MapAuth(this WebApplication app)
    {
        app.MapPost("/auth/login", async (IAuthAppService authAppService, AuthenticationRequest request) =>
        {
            var result = await authAppService.LoginAsync(request);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Unauthorized();
        });
    }
}
