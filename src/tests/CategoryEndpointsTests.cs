using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Api.Tests;

public class CategoryEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CategoryEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_ReturnsUnauthorized_WithoutToken()
    {
        var response = await _client.GetAsync("/api/v1/categories");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCategories_ReturnsOk_WhenAuthenticated()
    {
        var token = await GetAccessTokenAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/categories");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(payload));
    }

    [Fact]
    public async Task GetCategoryById_ReturnsOk_WhenExists()
    {
        var token = await GetAccessTokenAsync();
        var knownId = "20000000-0000-0000-0000-000000000001"; // Seeded Work category

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/categories/{knownId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(category);
        Assert.Equal("Work", category!.Name);
    }

    [Fact]
    public async Task GetCategoryById_ReturnsNotFound_WhenNonExistent()
    {
        var token = await GetAccessTokenAsync();
        var nonExistentId = Guid.NewGuid();

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/categories/{nonExistentId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreated_WhenValid()
    {
        var token = await GetAccessTokenAsync();
        var payload = new
        {
            Name = "Urgent Tasks",
            Description = "High priority tasks",
            Color = "#EF4444",
            Icon = "flame"
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(created);
        Assert.Equal("Urgent Tasks", created!.Name);
        Assert.Equal("#EF4444", created.Color);
        Assert.Equal("flame", created.Icon);
    }

    [Fact]
    public async Task CreateCategory_ReturnsValidationProblem_WhenNameIsMissing()
    {
        var token = await GetAccessTokenAsync();
        var payload = new
        {
            Name = "", // Invalid: Required and MinimumLength = 2
            Description = "No name",
            Color = "#000000",
            Icon = "x"
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("errors", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsOk_WhenValid()
    {
        var token = await GetAccessTokenAsync();

        // Create one first
        var createPayload = new
        {
            Name = "To Update",
            Description = "Before update",
            Color = "#123456",
            Icon = "pencil"
        };
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories")
        {
            Content = JsonContent.Create(createPayload)
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(created);

        // Update it
        var updatePayload = new
        {
            Name = "Updated Category",
            Description = "After update",
            Color = "#654321",
            Icon = "check"
        };
        using var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/categories/{created!.Id}")
        {
            Content = JsonContent.Create(updatePayload)
        };
        updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateResponse = await _client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(updated);
        Assert.Equal("Updated Category", updated!.Name);
        Assert.Equal("After update", updated.Description);
        Assert.Equal("#654321", updated.Color);
    }

    [Fact]
    public async Task DeleteCategory_ReturnsNoContent_WhenExists()
    {
        var token = await GetAccessTokenAsync();

        // Create one first
        var createPayload = new
        {
            Name = "To Delete",
            Description = "Will be deleted",
            Color = "#000000",
            Icon = "trash"
        };
        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/categories")
        {
            Content = JsonContent.Create(createPayload)
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createResponse = await _client.SendAsync(createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(created);

        // Delete it
        using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/categories/{created!.Id}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var deleteResponse = await _client.SendAsync(deleteRequest);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify it is gone
        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/categories/{created.Id}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var getResponse = await _client.SendAsync(getRequest);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            Email = "admin@example.com",
            Password = "Password123"
        });

        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.True(auth!.Success);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
        return auth.Token!;
    }

    private sealed class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
    }

    private sealed class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
