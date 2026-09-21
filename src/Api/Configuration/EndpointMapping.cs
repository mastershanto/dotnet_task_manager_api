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
    public static IEndpointRouteBuilder MapApiV1(this IEndpointRouteBuilder app)
    {
        var v1 = app.MapGroup("/api/v1");

        v1.MapAuth();
        v1.MapUsers();
        v1.MapProducts();
        v1.MapCategories();
        v1.MapPayment();
        v1.MapTasks();
        v1.MapProjects();

        return app;
    }

    public static IEndpointRouteBuilder MapLegacyRoutes(this IEndpointRouteBuilder app)
    {
        // Keep legacy routes for backward compatibility during migration.
        app.MapAuth();
        app.MapUsers();
        app.MapProducts();
        app.MapCategories();
        app.MapPayment();
        app.MapTasks();
        app.MapProjects();

        return app;
    }
}
