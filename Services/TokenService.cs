using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;

namespace TodoApi.Services;

/// <summary>
/// JWT settings configuration model (strongly-typed).
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";
    
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiresMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDays { get; set; } = 7;
    public int ClockSkewSeconds { get; set; } = 300;

    /// <summary>
    /// Validates JWT settings on configuration binding.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("JWT Issuer is not configured.");
        
        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("JWT Audience is not configured.");
        
        if (string.IsNullOrWhiteSpace(Key))
            throw new InvalidOperationException("JWT Key is not configured. Use secure secret management (environment variables or Azure Key Vault).");
        
        var keyBytes = Encoding.UTF8.GetBytes(Key);
        if (keyBytes.Length < 32)
            throw new InvalidOperationException("JWT symmetric key must be at least 256 bits (32 bytes) for HMAC-SHA256.");
        
        if (ExpiresMinutes < 1 || ExpiresMinutes > 1440)
            throw new InvalidOperationException("JWT ExpiresMinutes must be between 1 and 1440 (24 hours).");

        if (RefreshTokenExpiryDays < 1 || RefreshTokenExpiryDays > 90)
            throw new InvalidOperationException("RefreshTokenExpiryDays must be between 1 and 90 days.");
    }
}

/// <summary>
/// Token service contract for generating JWT tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Creates a JWT access token for the authenticated user.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <returns>Token string and expiration timestamp (UTC).</returns>
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}

/// <summary>
/// Enterprise-grade JWT token service implementing security best practices:
/// - Strongly-typed configuration with validation
/// - Structured logging for security audit trails
/// - Standard JWT claims (jti, iat, nbf) for token tracking and validation
/// - UTC timestamps to prevent timezone-related vulnerabilities
/// - Separation of concerns with private helper methods
/// - Comprehensive exception handling
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly SigningCredentials _signingCredentials;
    private readonly ILogger<TokenService> _logger;

    /// <summary>
    /// Initializes the TokenService with validated configuration and signing credentials.
    /// </summary>
    /// <param name="options">JWT settings from configuration (validated at startup).</param>
    /// <param name="logger">Logger for security audit trails.</param>
    /// <exception cref="ArgumentNullException">Thrown when options or logger is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when JWT configuration is invalid.</exception>
    public TokenService(IOptions<JwtSettings> options, ILogger<TokenService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jwtSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));

        // Validate JWT settings at construction
        try
        {
            _jwtSettings.Validate();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical(ex, "JWT configuration validation failed during TokenService initialization.");
            throw;
        }

        // Initialize signing credentials (cached for performance)
        try
        {
            var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            var signingKey = new SymmetricSecurityKey(keyBytes);
            _signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            
            _logger.LogInformation("TokenService initialized successfully with issuer: {Issuer}", _jwtSettings.Issuer);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to initialize JWT signing credentials.");
            throw new InvalidOperationException("Failed to initialize JWT signing credentials.", ex);
        }
    }

    /// <summary>
    /// Creates a JWT access token with standard claims and role-based authorization.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <returns>JWT token string and UTC expiration timestamp.</returns>
    /// <exception cref="ArgumentNullException">Thrown when user is null.</exception>
    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        if (user == null)
        {
            _logger.LogError("Attempted to create token for null user.");
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        try
        {
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(_jwtSettings.ExpiresMinutes);

            var claims = BuildClaims(user, now);
            var token = BuildJwtToken(claims, now, expiresAt);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation(
                "JWT token created for user {UserId} (Username: {Username}). Expires at {ExpiresAt} UTC.",
                user.Id, user.Username, expiresAt);

            return (tokenString, expiresAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create JWT token for user {UserId}.", user.Id);
            throw new InvalidOperationException($"Failed to generate token for user {user.Id}.", ex);
        }
    }

    /// <summary>
    /// Builds a collection of standard and custom claims for the JWT token.
    /// Uses standard JWT registered claims for interoperability.
    /// </summary>
    private List<Claim> BuildClaims(User user, DateTime issuedAt)
    {
        var claims = new List<Claim>
        {
            // Standard JWT registered claims (RFC 7519)
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),                    // Subject (user ID)
            new(JwtRegisteredClaimNames.UniqueName, user.Username ?? string.Empty),  // Unique name
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),          // Email
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),             // JWT ID (unique token identifier)
            new(JwtRegisteredClaimNames.Iat,                                          // Issued at timestamp
                new DateTimeOffset(issuedAt).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64)
        };

        // Add role claims for authorization
        if (user.Roles != null && user.Roles.Length > 0)
        {
            foreach (var role in user.Roles)
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            
            _logger.LogDebug("Added {RoleCount} role claims for user {UserId}.", user.Roles.Length, user.Id);
        }

        return claims;
    }

    /// <summary>
    /// Builds a JwtSecurityToken with configured issuer, audience, and signing credentials.
    /// Includes NotBefore claim to prevent token use before issuance.
    /// </summary>
    private JwtSecurityToken BuildJwtToken(List<Claim> claims, DateTime notBefore, DateTime expiresAt)
    {
        return new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: notBefore,      // Token not valid before this time (prevents premature use)
            expires: expiresAt,         // Token expiration time
            signingCredentials: _signingCredentials);
    }
}
