using Auth.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Data;

public class AuthService : IAuthService
{
    private readonly JwtOptions _jwtOptions;
    private static readonly ConcurrentDictionary<string, (RegisterRequest Req, string Otp, DateTime Expiry)> _pendingRegistrations = new();
    private static readonly ConcurrentDictionary<string, (string Otp, string? ResetToken, DateTime Expiry)> _pendingResets = new();
    private static readonly ConcurrentDictionary<string, AuthUserData> _users = new();
    private static int _nextId = 1;

    public AuthService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;

        // Seed demo users
        if (_users.IsEmpty)
        {
            SeedUser("Admin User", "admin@example.com", "Password123", "admin");
            SeedUser("Student Demo", "student@gmail.com", "password", "student");
            SeedUser("Instructor Demo", "instructor@gmail.com", "password", "instructor");
            SeedUser("Renter Demo", "renter@gmail.com", "password", "renter");
        }
    }

    private void SeedUser(string name, string email, string password, string type)
    {
        var id = Interlocked.Increment(ref _nextId);
        _users[email.ToLowerInvariant()] = new AuthUserData
        {
            Id = id, Name = name, Email = email, Type = type, IsFullProgram = true
        };
    }

    public Task<AuthResult> AuthenticateAsync(AuthenticationRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (_users.TryGetValue(email, out var user))
        {
            var token = GenerateJwt(user);
            return Task.FromResult(new AuthResult(true, "Login successful", token));
        }
        return Task.FromResult(new AuthResult(false, "Invalid credentials", null));
    }

    public Task<RegisterResult> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (_users.ContainsKey(email))
            return Task.FromResult(new RegisterResult(false, "Email already registered", null));

        var otp = GenerateOtp();
        _pendingRegistrations[email] = (request, otp, DateTime.UtcNow.AddMinutes(10));
        return Task.FromResult(new RegisterResult(true, $"OTP sent to {email}. Use OTP: {otp}", null));
    }

    public Task<OtpResult> VerifyOtpAsync(OtpRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (!_pendingRegistrations.TryGetValue(email, out var pending))
            return Task.FromResult(new OtpResult(false, "No pending registration found", null));

        if (DateTime.UtcNow > pending.Expiry)
        {
            _pendingRegistrations.TryRemove(email, out _);
            return Task.FromResult(new OtpResult(false, "OTP has expired", null));
        }

        if (pending.Otp != request.Otp)
            return Task.FromResult(new OtpResult(false, "Invalid OTP", null));

        var id = Interlocked.Increment(ref _nextId);
        var user = new AuthUserData
        {
            Id = id, Name = pending.Req.Name, Email = pending.Req.Email,
            Type = pending.Req.Type, IsFullProgram = pending.Req.IsFullProgram
        };
        _users[email] = user;
        _pendingRegistrations.TryRemove(email, out _);

        var token = GenerateJwt(user);
        var userData = user with { AccessToken = token };
        return Task.FromResult(new OtpResult(true, "Registration verified successfully", userData));
    }

    public Task<RegisterResult> ResendOtpAsync(PasswordResetRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (!_pendingRegistrations.TryGetValue(email, out var pending))
            return Task.FromResult(new RegisterResult(false, "No pending registration found", null));

        var otp = GenerateOtp();
        _pendingRegistrations[email] = (pending.Req, otp, DateTime.UtcNow.AddMinutes(10));
        return Task.FromResult(new RegisterResult(true, $"OTP resent. Use OTP: {otp}", null));
    }

    public Task<PasswordResetResult> PasswordResetRequestAsync(PasswordResetRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (!_users.ContainsKey(email))
            return Task.FromResult(new PasswordResetResult(false, "Email not found", null));

        var otp = GenerateOtp();
        _pendingResets[email] = (otp, null, DateTime.UtcNow.AddMinutes(10));
        return Task.FromResult(new PasswordResetResult(true, $"Password reset OTP sent. Use OTP: {otp}", null));
    }

    public Task<PasswordResetResult> PasswordResetResendAsync(PasswordResetRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (!_pendingResets.ContainsKey(email))
            return Task.FromResult(new PasswordResetResult(false, "No pending reset found", null));

        var otp = GenerateOtp();
        _pendingResets[email] = (otp, null, DateTime.UtcNow.AddMinutes(10));
        return Task.FromResult(new PasswordResetResult(true, $"OTP resent. Use OTP: {otp}", null));
    }

    public Task<PasswordResetResult> PasswordResetVerifyAsync(PasswordResetVerifyRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (!_pendingResets.TryGetValue(email, out var pending))
            return Task.FromResult(new PasswordResetResult(false, "No pending reset found", null));

        if (DateTime.UtcNow > pending.Expiry)
        {
            _pendingResets.TryRemove(email, out _);
            return Task.FromResult(new PasswordResetResult(false, "OTP has expired", null));
        }

        if (pending.Otp != request.Otp)
            return Task.FromResult(new PasswordResetResult(false, "Invalid OTP", null));

        var resetToken = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            Encoding.UTF8.GetBytes($"{email}{DateTime.UtcNow.Ticks}"))).ToLowerInvariant();
        _pendingResets[email] = (pending.Otp, resetToken, pending.Expiry);
        return Task.FromResult(new PasswordResetResult(true, "OTP verified", resetToken));
    }

    public Task<PasswordResetResult> PasswordResetConfirmAsync(PasswordResetConfirmRequest request)
    {
        var entry = _pendingResets.FirstOrDefault(x => x.Value.ResetToken == request.ResetToken);
        if (string.IsNullOrEmpty(entry.Key))
            return Task.FromResult(new PasswordResetResult(false, "Invalid or expired reset token", null));

        _pendingResets.TryRemove(entry.Key, out _);
        return Task.FromResult(new PasswordResetResult(true, "Password has been reset successfully", null));
    }

    private string GenerateJwt(AuthUserData user)
    {
        var now = DateTime.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Type),
            new("user_type", user.Type),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_jwtOptions.TokenExpirationMinutes),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateOtp() => Random.Shared.Next(1000, 9999).ToString();
}
