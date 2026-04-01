using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Api.Configuration;

public static class TelemetryConfiguration
{
    public static IServiceCollection AddApiOpenTelemetry(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var serviceName = configuration["OpenTelemetry:ServiceName"] ?? "dotnet-task-manager-api";
        var otlpEndpoint = configuration["OpenTelemetry:OtlpEndpoint"];
        var consoleEnabled = bool.TryParse(configuration["OpenTelemetry:ConsoleExporterEnabled"], out var configured) ? configured : environment.IsDevelopment();

        var openTelemetry = services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName: serviceName));

        openTelemetry.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            tracing.AddSource("Npgsql");

            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                tracing.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
            }

            if (consoleEnabled)
            {
                tracing.AddConsoleExporter();
            }
        });

        openTelemetry.WithMetrics(metrics =>
        {
            metrics.AddAspNetCoreInstrumentation();
            metrics.AddHttpClientInstrumentation();
            metrics.AddRuntimeInstrumentation();

            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                metrics.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
            }

            if (consoleEnabled)
            {
                metrics.AddConsoleExporter();
            }
        });

        return services;
    }
}
