using Auth.Application.Features.Auth.Commands.Login;
using Auth.Domain;
using BuildingBlocks.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auth.Presentation;

public static class AuthEndpoints
{
    public static void MapAuth(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth").WithTags("Auth").AllowAnonymous();

        // POST /auth/login (COMMAND)
        group.MapPost("/login", async (ISender sender, AuthenticationRequest request, CancellationToken cancellationToken) =>
        {
            var validation = Validation.Validate(request).ToArray();
            if (validation.Any())
            {
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));
            }

            var command = new LoginCommand(request.Email, request.Password);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Unauthorized();
        })
        .Produces<AuthResult>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
