using Auth.Presentation;
using Categories.Presentation;
using Payments.Presentation;
using Products.Presentation;
using Projects.Presentation;
using Tasks.Presentation;
using Users.Presentation;

namespace Api.Configuration;

public static class EndpointMapping
{
    public static IEndpointRouteBuilder MapModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapUserEndpoints();
        app.MapProductEndpoints();
        app.MapCategoryEndpoints();
        app.MapPaymentEndpoints();
        app.MapTaskEndpoints();
        app.MapProjectEndpoints();

        return app;
    }

    public static IEndpointRouteBuilder MapApiV1(this IEndpointRouteBuilder app)
    {
        var v1 = app.MapGroup("/api/v1");
        v1.MapModuleEndpoints();
        return app;
    }

    public static IEndpointRouteBuilder MapLegacyRoutes(this IEndpointRouteBuilder app)
    {
        // Keep legacy routes for backward compatibility during migration.
        app.MapModuleEndpoints();
        return app;
    }
}
