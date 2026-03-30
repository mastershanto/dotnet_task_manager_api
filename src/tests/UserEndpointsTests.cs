using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Api.Tests;

public class UserEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UserEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthLive_ReturnsOk()
    {
        var response = await _client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_ReturnsOk()
    {
        var response = await _client.GetAsync("/users");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateUser_ReturnsCreated_WhenValid()
    {
        var payload = new { Name = "Test", Email = "test@example.com" };
        var response = await _client.PostAsJsonAsync("/users", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
