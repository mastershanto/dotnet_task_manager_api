namespace TodoApi.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "TodoApi";
    public string Audience { get; set; } = "TodoApi";

    // NOTE: For production, store this in a secret store and rotate regularly.
    public string SigningKey { get; set; } = "CHANGE_ME_TO_A_LONG_RANDOM_SECRET";

    // Minutes
    public int AccessTokenLifetimeMinutes { get; set; } = 60;
}
