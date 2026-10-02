using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Auth.Application.Models;
using Auth.Domain;
using Auth.Presentation;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Api.Tests;

public class AuthEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_And_VerifyOtp_FullFlow_Succeeds()
    {
        var testEmail = $"testuser_{Guid.NewGuid():N}@example.com";

        // 1. Register (Sign-up)
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            Name = "New Test User",
            Email = testEmail,
            Password = "SecurePassword123"
        });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
        var regResult = await registerResponse.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(regResult);
        Assert.True(regResult!.Success);
        Assert.False(string.IsNullOrWhiteSpace(regResult.Otp));

        // 2. Verify Registration OTP
        var verifyResponse = await _client.PostAsJsonAsync("/api/v1/auth/verify-registration-otp", new
        {
            Email = testEmail,
            Otp = regResult.Otp
        });

        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);
        var authResult = await verifyResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResult);
        Assert.True(authResult!.Success);
        Assert.False(string.IsNullOrWhiteSpace(authResult.Token));

        // 3. Login with newly verified user
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            Email = testEmail,
            Password = "SecurePassword123"
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginResult);
        Assert.True(loginResult!.Success);
        Assert.False(string.IsNullOrWhiteSpace(loginResult.Token));

        // 4. View Profile using the token
        using var profileRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/profile");
        profileRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        var profileResponse = await _client.SendAsync(profileRequest);
        Assert.Equal(HttpStatusCode.OK, profileResponse.StatusCode);
        var profile = await profileResponse.Content.ReadFromJsonAsync<UserProfileResponse>();
        Assert.NotNull(profile);
        Assert.Equal("New Test User", profile!.Name);
        Assert.Equal(testEmail, profile.Email);

        // 5. Update Profile (Name)
        using var updateRequest = new HttpRequestMessage(HttpMethod.Put, "/api/v1/auth/profile")
        {
            Content = JsonContent.Create(new UpdateProfileRequest("Updated Test User"))
        };
        updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        var updateResponse = await _client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedProfile = await updateResponse.Content.ReadFromJsonAsync<UserProfileResponse>();
        Assert.NotNull(updatedProfile);
        Assert.Equal("Updated Test User", updatedProfile!.Name);

        // 6. Change Password
        using var changePasswordRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/change-password")
        {
            Content = JsonContent.Create(new ChangePasswordRequest("SecurePassword123", "NewSecurePassword456"))
        };
        changePasswordRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        var changePasswordResponse = await _client.SendAsync(changePasswordRequest);
        Assert.Equal(HttpStatusCode.OK, changePasswordResponse.StatusCode);

        // 7. Login with old password should fail, new password should succeed
        var oldLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            Email = testEmail,
            Password = "SecurePassword123"
        });
        Assert.Equal(HttpStatusCode.Unauthorized, oldLoginResponse.StatusCode);

        var newLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            Email = testEmail,
            Password = "NewSecurePassword456"
        });
        Assert.Equal(HttpStatusCode.OK, newLoginResponse.StatusCode);
        var newAuth = await newLoginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // 8. Delete Account
        using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/v1/auth/account")
        {
            Content = JsonContent.Create(new DeleteAccountRequest("NewSecurePassword456"))
        };
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAuth!.Token);

        var deleteResponse = await _client.SendAsync(deleteRequest);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_And_ResetPassword_Flow_Succeeds()
    {
        var testEmail = $"forgot_{Guid.NewGuid():N}@example.com";

        // Register and verify
        var reg = await _client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            Name = "Forgot Password User",
            Email = testEmail,
            Password = "InitialPassword123"
        });
        var regData = await reg.Content.ReadFromJsonAsync<RegisterResponse>();

        await _client.PostAsJsonAsync("/api/v1/auth/verify-registration-otp", new
        {
            Email = testEmail,
            Otp = regData!.Otp
        });

        // 1. Forgot password request
        var forgotResponse = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", new
        {
            Email = testEmail
        });

        Assert.Equal(HttpStatusCode.OK, forgotResponse.StatusCode);
        var forgotData = await forgotResponse.Content.ReadFromJsonAsync<ForgotPasswordResponse>();
        Assert.NotNull(forgotData);
        Assert.True(forgotData!.Success);
        Assert.False(string.IsNullOrWhiteSpace(forgotData.Otp));

        // 2. Verify Reset OTP
        var verifyResetResponse = await _client.PostAsJsonAsync("/api/v1/auth/verify-reset-otp", new
        {
            Email = testEmail,
            Otp = forgotData.Otp
        });
        Assert.Equal(HttpStatusCode.OK, verifyResetResponse.StatusCode);

        // 3. Reset Password
        var resetResponse = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", new
        {
            Email = testEmail,
            Otp = forgotData.Otp,
            NewPassword = "BrandNewPassword999"
        });
        Assert.Equal(HttpStatusCode.OK, resetResponse.StatusCode);

        // 4. Verify login works with new password
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            Email = testEmail,
            Password = "BrandNewPassword999"
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [Fact]
    public async Task Logout_ReturnsOk()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            Email = "admin@example.com",
            Password = "Password123"
        });
        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/logout");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
