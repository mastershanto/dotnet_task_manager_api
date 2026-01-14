using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TodoApi.IntegrationTests;

public class HealthEndpointTests
{
    [Fact]
    public async Task LiveHealth_Returns_Healthy()
    {
        using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Testing"));

        using var client = app.CreateClient();
        var res = await client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", body);
    }
}
