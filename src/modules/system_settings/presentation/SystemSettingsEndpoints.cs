using BuildingBlocks.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SystemSettings.Application.Features.SystemSettings.Queries.GetSystemSettingByKey;
using SystemSettings.Application.Features.SystemSettings.Queries.GetSystemSettings;
using SystemSettings.Domain;

namespace SystemSettings.Presentation;

public static class SystemSettingsEndpoints
{
    public static void MapSystemSettings(this IEndpointRouteBuilder endpoints)
    {
        MapRoutes(endpoints.MapGroup("/system").WithTags("System").AllowAnonymous());
        MapRoutes(endpoints.MapGroup("/api/system").WithTags("System").AllowAnonymous());
    }

    private static void MapRoutes(RouteGroupBuilder group)
    {
        // GET /system or /api/system
        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSystemSettingsQuery(), ct);
            var dict = result.Value?.ToDictionary(s => s.Key, s => s.Value) ?? new Dictionary<string, string>();
            return Results.Ok(new { status = true, message = "Success", data = dict, code = 200 });
        })
        .Produces(StatusCodes.Status200OK);

        // GET /system/app-version
        group.MapGet("/app-version", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSystemSettingByKeyQuery("app-version"), ct);
            var version = result.Value?.Value ?? "1.0.0";
            return Results.Ok(new AppVersionResponse(true, "App version retrieved successfully", new AppVersionData(version)));
        })
        .Produces<AppVersionResponse>(StatusCodes.Status200OK);

        // GET /system/terms-of-service
        group.MapGet("/terms-of-service", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSystemSettingByKeyQuery("terms-of-service"), ct);
            var url = result.Value?.Value ?? "https://sunsetdance.thewarriors.team/terms-of-service";
            return Results.Ok(new TermsOfServiceResponse(true, "Terms of service retrieved successfully", new TermsOfServiceData(url, url)));
        })
        .Produces<TermsOfServiceResponse>(StatusCodes.Status200OK);

        // GET /system/privacy-policy
        group.MapGet("/privacy-policy", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSystemSettingByKeyQuery("privacy-policy"), ct);
            var url = result.Value?.Value ?? "https://sunsetdance.thewarriors.team/privacy-policy";
            return Results.Ok(new PrivacyPolicyResponse(true, "Privacy policy retrieved successfully", new PrivacyPolicyData(url, url)));
        })
        .Produces<PrivacyPolicyResponse>(StatusCodes.Status200OK);

        // GET /system/site.title
        group.MapGet("/site.title", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSystemSettingByKeyQuery("site.title"), ct);
            var val = result.Value?.Value ?? "Sunset Dance Center";
            return Results.Ok(new SystemGenericResponse(true, "Success", new { title = val }));
        })
        .Produces<SystemGenericResponse>(StatusCodes.Status200OK);

        // GET /system/site.logo
        group.MapGet("/site.logo", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSystemSettingByKeyQuery("site.logo"), ct);
            var val = result.Value?.Value ?? "https://sunsetdance.thewarriors.team/assets/images/logo.png";
            return Results.Ok(new SystemGenericResponse(true, "Success", new { logo = val }));
        })
        .Produces<SystemGenericResponse>(StatusCodes.Status200OK);

        // GET /system/profile-message
        group.MapGet("/profile-message", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSystemSettingByKeyQuery("profile-message"), ct);
            var val = result.Value?.Value ?? "Welcome to Sunset Dance Center!";
            return Results.Ok(new SystemGenericResponse(true, "Success", new { message = val }));
        })
        .Produces<SystemGenericResponse>(StatusCodes.Status200OK);
    }
}
