using App.Features.Auth.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Features.Auth.Data;

public class AuthService : IAuthService
{
    private const string DemoEmail = "admin@example.com";
    private const string DemoPassword = "Password123";
    private readonly JwtOptions _jwtOptions;

    public AuthService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public Task<AuthResult> AuthenticateAsync(AuthenticationRequest request)
    {
        if (request.Email.Equals(DemoEmail, StringComparison.OrdinalIgnoreCase) && request.Password == DemoPassword)
        {
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(_jwtOptions.TokenExpirationMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, request.Email),
                new(JwtRegisteredClaimNames.Email, request.Email),
                new(ClaimTypes.Name, request.Email),
                new(ClaimTypes.Role, "admin"),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: signingCredentials);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(new AuthResult(true, "Logged in", tokenValue));
        }

        return Task.FromResult(new AuthResult(false, "Invalid credentials", null));
    }
}
