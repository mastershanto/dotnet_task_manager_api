using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;

namespace TodoApi.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        var jwt = _configuration.GetSection("Jwt");
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;
        var key = jwt["Key"]!;
        var expiresMinutes = int.TryParse(jwt["ExpiresMinutes"], out var parsed) ? parsed : 60;

        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles ?? Array.Empty<string>())
            claims.Add(new Claim(ClaimTypes.Role, role));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
