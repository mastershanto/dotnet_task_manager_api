using App.Features.Auth.Application;
using App.Features.Auth.Domain;
using Shared;

namespace App.Features.Auth.Presentation;

public static class AuthEndpoints
{
    public static void MapAuth(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth").WithTags("Auth").AllowAnonymous();

        group.MapPost("/login", async (IAuthAppService authAppService, AuthenticationRequest request) =>
        {
            var validation = Validation.Validate(request).ToArray();
            if (validation.Any())
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));

            var result = await authAppService.LoginAsync(request);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Unauthorized();
        })
        .Produces<AuthResult>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
