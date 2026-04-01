namespace Auth.Domain;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "TaskManager.Api";
    public string Audience { get; set; } = "TaskManager.Client";
    public string SigningKey { get; set; } = "this-is-a-dev-only-signing-key-change-in-production-12345";
    public int TokenExpirationMinutes { get; set; } = 60;
}
