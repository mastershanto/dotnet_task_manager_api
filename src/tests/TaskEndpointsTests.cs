using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Tasks.Domain;
using Xunit;

namespace Api.Tests;

public class TaskEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TaskEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTasks_ReturnsUnauthorized_WithoutToken()
    {
        var response = await _client.GetAsync("/api/v1/tasks");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTasks_ReturnsOk_WhenAuthenticated()
    {
        var token = await GetAccessTokenAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/tasks");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<TaskDto>>();
        Assert.NotNull(tasks);
    }

    [Fact]
    public async Task GetTaskById_ReturnsOk_WhenExists()
    {
        var token = await GetAccessTokenAsync();
        var knownId = "30000000-0000-0000-0000-000000000001"; // Seeded task

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/tasks/{knownId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var task = await response.Content.ReadFromJsonAsync<TaskDto>();
        Assert.NotNull(task);
        Assert.Equal("Initial Project Setup", task!.Title);
    }

    [Fact]
    public async Task GetTaskById_ReturnsNotFound_WhenNonExistent()
    {
        var token = await GetAccessTokenAsync();
        var nonExistentId = Guid.NewGuid();

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/tasks/{nonExistentId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_ReturnsCreated_WhenValid()
    {
        var token = await GetAccessTokenAsync();
        var payload = new
        {
            Title = "Complete CQRS Pipeline",
            Description = "Implement commands, queries, and validators",
            Status = TaskItemStatus.InProgress,
            Priority = TaskPriority.High,
            CategoryId = Guid.Parse("20000000-0000-0000-0000-000000000001")
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/tasks")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<TaskDto>();
        Assert.NotNull(created);
        Assert.Equal("Complete CQRS Pipeline", created!.Title);
        Assert.Equal(TaskItemStatus.InProgress, created.Status);
    }

    [Fact]
    public async Task CreateTask_ReturnsValidationProblem_WhenTitleIsTooShort()
    {
        var token = await GetAccessTokenAsync();
        var payload = new
        {
            Title = "AB", // Less than 3 characters, fails validation
            Description = "Short title test"
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/tasks")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_ReturnsOk_WhenValid()
    {
        var token = await GetAccessTokenAsync();

        // 1. Create a task first
        var createPayload = new
        {
            Title = "Task to be Updated",
            Description = "Before update",
            Status = TaskItemStatus.Todo,
            Priority = TaskPriority.Low
        };

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/tasks")
        {
            Content = JsonContent.Create(createPayload)
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createResponse = await _client.SendAsync(createRequest);
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<TaskDto>();
        Assert.NotNull(created);

        // 2. Update the task
        var updatePayload = new
        {
            Title = "Updated Task Title",
            Description = "After update",
            Status = TaskItemStatus.Completed,
            Priority = TaskPriority.High,
            DueDate = (DateTime?)null,
            CategoryId = (Guid?)null,
            AssignedUserId = (Guid?)null
        };

        using var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/tasks/{created!.Id}")
        {
            Content = JsonContent.Create(updatePayload)
        };
        updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var updateResponse = await _client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<TaskDto>();
        Assert.NotNull(updated);
        Assert.Equal("Updated Task Title", updated!.Title);
        Assert.Equal(TaskItemStatus.Completed, updated.Status);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNoContent_WhenExists()
    {
        var token = await GetAccessTokenAsync();

        // 1. Create a task
        var createPayload = new
        {
            Title = "Task to be Deleted",
            Description = "Will be removed soon",
            Status = TaskItemStatus.Todo,
            Priority = TaskPriority.Low
        };

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/tasks")
        {
            Content = JsonContent.Create(createPayload)
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createResponse = await _client.SendAsync(createRequest);
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<TaskDto>();
        Assert.NotNull(created);

        // 2. Delete it
        using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/tasks/{created!.Id}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var deleteResponse = await _client.SendAsync(deleteRequest);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // 3. Verify it is gone
        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/tasks/{created.Id}");
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

    private sealed class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
