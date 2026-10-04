namespace Auth.Domain;

public record RegisterRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Type { get; init; } = "student"; // student | instructor | renter
    public bool AgreeToTerms { get; init; }
    public bool IsFullProgram { get; init; }
}

public record OtpRequest
{
    public string Email { get; init; } = string.Empty;
    public string? Otp { get; init; }
}

public record PasswordResetRequest
{
    public string Email { get; init; } = string.Empty;
}

public record PasswordResetVerifyRequest
{
    public string Email { get; init; } = string.Empty;
    public string Otp { get; init; } = string.Empty;
}

public record PasswordResetConfirmRequest
{
    public string ResetToken { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record RegisterResult(bool Status, string Message, AuthUserData? Data);
public record OtpResult(bool Status, string Message, AuthUserData? Data);
public record PasswordResetResult(bool Status, string Message, string? ResetToken);

public record AuthUserData
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string? AccessToken { get; init; }
    public string? Avatar { get; init; }
    public bool IsFullProgram { get; init; }
}
