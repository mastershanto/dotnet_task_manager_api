using Auth.Presentation;
using Payments.Presentation;
using Products.Presentation;
using Users.Presentation;

namespace Api.Configuration;

public static class EndpointMapping
{
    public static IEndpointRouteBuilder MapApiV1(this IEndpointRouteBuilder app)
    {
        var v1 = app.MapGroup("/api/v1");

        v1.MapAuth();
        v1.MapUsers();
        v1.MapProducts();
        v1.MapPayment();

        return app;
    }

    public static IEndpointRouteBuilder MapLegacyRoutes(this IEndpointRouteBuilder app)
    {
        // Keep legacy routes for backward compatibility during migration.
        app.MapAuth();
        app.MapUsers();
        app.MapProducts();
        app.MapPayment();

        return app;
    }
}
