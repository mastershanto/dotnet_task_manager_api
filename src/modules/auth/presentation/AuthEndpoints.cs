using System.Security.Claims;
using Auth.Application.Features.Auth.Commands.ChangePassword;
using Auth.Application.Features.Auth.Commands.DeleteAccount;
using Auth.Application.Features.Auth.Commands.ForgotPassword;
using Auth.Application.Features.Auth.Commands.Login;
using Auth.Application.Features.Auth.Commands.Register;
using Auth.Application.Features.Auth.Commands.ResetPassword;
using Auth.Application.Features.Auth.Commands.UpdateProfile;
using Auth.Application.Features.Auth.Commands.VerifyRegistrationOtp;
using Auth.Application.Features.Auth.Commands.VerifyResetOtp;
using Auth.Application.Features.Auth.Queries.GetProfile;
using Auth.Domain;
using BuildingBlocks.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auth.Presentation;

// Profile Endpoint Request Records
public record UpdateProfileRequest(string Name);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record DeleteAccountRequest(string Password);

/// <summary>
/// প্রমাণীকরণ ও প্রোফাইল API এন্ডপয়েন্ট (Clean Architecture Presentation Layer):
/// এন্ডপয়েন্টগুলো সরাসরি কোনো লজিক রাখে না, বরং MediatR (ISender)-এর মাধ্যমে কমান্ড ও কুয়েরিতে রাউট করে।
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuth(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth").WithTags("Authentication & Profile");

        // ==========================================
        // ১. উন্মুক্ত এন্ডপয়েন্ট (Public / Anonymous)
        // ==========================================

        // POST /auth/register
        group.MapPost("/register", async (RegisterCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .AllowAnonymous()
        .WithSummary("নতুন ইউজার সাইন-আপ এবং ওটিপি জেনারেশন")
        .Produces<RegisterResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /auth/verify-registration-otp
        group.MapPost("/verify-registration-otp", async (VerifyRegistrationOtpCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .AllowAnonymous()
        .WithSummary("সাইন-আপ ওটিপি যাচাই ও অটো-লগইন (JWT টোকেন প্রাপ্তি)")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /auth/login
        group.MapPost("/login", async (LoginCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Json(new { errors = result.Errors }, statusCode: StatusCodes.Status401Unauthorized);
        })
        .AllowAnonymous()
        .WithSummary("ইমেইল ও পাসওয়ার্ড দিয়ে সিস্টেমে লগইন")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        // POST /auth/forgot-password
        group.MapPost("/forgot-password", async (ForgotPasswordCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .AllowAnonymous()
        .WithSummary("পাসওয়ার্ড ভুলে গেলে ইমেইলে ওটিপি প্রেরণ")
        .Produces<ForgotPasswordResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /auth/verify-reset-otp
        group.MapPost("/verify-reset-otp", async (VerifyResetOtpCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(new { message = "OTP verified successfully. You may now reset your password." })
                : Results.BadRequest(new { errors = result.Errors });
        })
        .AllowAnonymous()
        .WithSummary("পাসওয়ার্ড রিসেটের ওটিপি সঠিক কিনা তা যাচাই")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /auth/reset-password
        group.MapPost("/reset-password", async (ResetPasswordCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(new { message = "Password reset successfully. You can now login with your new password." })
                : Results.BadRequest(new { errors = result.Errors });
        })
        .AllowAnonymous()
        .WithSummary("ওটিপি কোড দিয়ে নতুন পাসওয়ার্ড সেট করা")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);


        // ==========================================
        // ২. সুরক্ষিত প্রোফাইল এন্ডপয়েন্ট (Authorized)
        // ==========================================
        var protectedGroup = group.MapGroup("/")
            .RequireAuthorization(AuthPolicies.ApiUser);

        // GET /auth/profile
        protectedGroup.MapGet("/profile", async (ClaimsPrincipal userPrincipal, ISender sender, CancellationToken cancellationToken) =>
        {
            var userId = GetCurrentUserId(userPrincipal);
            if (userId is null) return Results.Unauthorized();

            var result = await sender.Send(new GetProfileQuery(userId.Value), cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { errors = result.Errors });
        })
        .WithSummary("বর্তমান লগইনকৃত ইউজারের প্রোফাইল দেখা")
        .Produces<UserProfileResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // PUT /auth/profile
        protectedGroup.MapPut("/profile", async ([Microsoft.AspNetCore.Mvc.FromBody] UpdateProfileRequest request, ClaimsPrincipal userPrincipal, ISender sender, CancellationToken cancellationToken) =>
        {
            var userId = GetCurrentUserId(userPrincipal);
            if (userId is null) return Results.Unauthorized();

            var command = new UpdateProfileCommand(userId.Value, request.Name);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .WithSummary("প্রোফাইল তথ্য (যেমন নাম) আপডেট করা")
        .Produces<UserProfileResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /auth/change-password
        protectedGroup.MapPost("/change-password", async ([Microsoft.AspNetCore.Mvc.FromBody] ChangePasswordRequest request, ClaimsPrincipal userPrincipal, ISender sender, CancellationToken cancellationToken) =>
        {
            var userId = GetCurrentUserId(userPrincipal);
            if (userId is null) return Results.Unauthorized();

            var command = new ChangePasswordCommand(userId.Value, request.CurrentPassword, request.NewPassword);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(new { message = "Password changed successfully." })
                : Results.BadRequest(new { errors = result.Errors });
        })
        .WithSummary("বর্তমান পাসওয়ার্ড যাচাই করে নতুন পাসওয়ার্ড সেট করা")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // DELETE /auth/account
        protectedGroup.MapDelete("/account", async ([Microsoft.AspNetCore.Mvc.FromBody] DeleteAccountRequest request, ClaimsPrincipal userPrincipal, ISender sender, CancellationToken cancellationToken) =>
        {
            var userId = GetCurrentUserId(userPrincipal);
            if (userId is null) return Results.Unauthorized();

            var command = new DeleteAccountCommand(userId.Value, request.Password);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(new { message = "Your account has been deleted permanently." })
                : Results.BadRequest(new { errors = result.Errors });
        })
        .WithSummary("পাসওয়ার্ড নিশ্চিতকরণ সাপেক্ষে একাউন্ট স্থায়ীভাবে ডিলিট করা")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /auth/logout
        protectedGroup.MapPost("/logout", () =>
        {
            // Stateless JWT টোকেনের ক্ষেত্রে ক্লায়েন্ট সাইড থেকে টোকেন ডিসকার্ড করাই আদর্শ প্র্যাকটিস
            return Results.Ok(new { message = "Logged out successfully. Please clear your local token." });
        })
        .WithSummary("সিস্টেম থেকে লগআউট করা")
        .Produces(StatusCodes.Status200OK);
    }

    private static Guid? GetCurrentUserId(ClaimsPrincipal principal)
    {
        var claimValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? principal.FindFirst("sub")?.Value;

        return Guid.TryParse(claimValue, out var id) ? id : null;
    }
}
