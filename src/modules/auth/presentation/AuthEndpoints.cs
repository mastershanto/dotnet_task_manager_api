using Auth.Application.Features.Auth.Commands.Login;
using Auth.Application.Features.Auth.Commands.PasswordReset;
using Auth.Application.Features.Auth.Commands.Register;
using Auth.Application.Features.Auth.Commands.ResendOtp;
using Auth.Application.Features.Auth.Commands.VerifyOtp;
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

        // POST /auth/login
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

        // POST /auth/register
        group.MapPost("/register", async (ISender sender, RegisterRequest request, CancellationToken cancellationToken) =>
        {
            var command = new RegisterCommand(
                request.Name,
                request.Email,
                request.Password,
                request.Type,
                request.AgreeToTerms,
                request.IsFullProgram
            );
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<RegisterResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /auth/register/resend-otp
        group.MapPost("/register/resend-otp", async (ISender sender, OtpRequest request, CancellationToken cancellationToken) =>
        {
            var command = new ResendOtpCommand(request.Email);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<RegisterResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /auth/register/verify-otp
        group.MapPost("/register/verify-otp", async (ISender sender, OtpRequest request, CancellationToken cancellationToken) =>
        {
            var command = new VerifyOtpCommand(request.Email, request.Otp ?? string.Empty);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<OtpResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /auth/password/reset/request
        group.MapPost("/password/reset/request", async (ISender sender, PasswordResetRequest request, CancellationToken cancellationToken) =>
        {
            var command = new PasswordResetRequestCommand(request.Email);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<PasswordResetResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /auth/password/reset/resend
        group.MapPost("/password/reset/resend", async (ISender sender, PasswordResetRequest request, CancellationToken cancellationToken) =>
        {
            var command = new PasswordResetResendCommand(request.Email);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<PasswordResetResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /auth/password/reset/verify
        group.MapPost("/password/reset/verify", async (ISender sender, PasswordResetVerifyRequest request, CancellationToken cancellationToken) =>
        {
            var command = new PasswordResetVerifyCommand(request.Email, request.Otp);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<PasswordResetResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /auth/password/reset/confirm
        group.MapPost("/password/reset/confirm", async (ISender sender, PasswordResetConfirmRequest request, CancellationToken cancellationToken) =>
        {
            var command = new PasswordResetConfirmCommand(request.ResetToken, request.Password);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<PasswordResetResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
