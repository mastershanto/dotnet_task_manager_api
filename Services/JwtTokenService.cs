using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;
using TodoApi.Options;

namespace TodoApi.Services;

public interface IJwtTokenService
{
    (string token, DateTime expiresAtUtc) CreateAccessToken(User user);
}

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public (string token, DateTime expiresAtUtc) CreateAccessToken(User user)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_options.AccessTokenLifetimeMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };

        foreach (var role in user.Roles ?? Array.Empty<string>())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: creds);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token, expiresAt);
    }
}
