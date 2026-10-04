namespace SystemSettings.Domain;

public record SystemSettingModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; init; }
}

public record AppVersionResponse(bool Status, string Message, AppVersionData Data, int Code = 200);
public record AppVersionData(string AppVersion = "1.0.0");

public record TermsOfServiceResponse(bool Status, string Message, TermsOfServiceData Data, int Code = 200);
public record TermsOfServiceData(string Link = "https://sunsetdance.thewarriors.team/terms-of-service", string Url = "https://sunsetdance.thewarriors.team/terms-of-service");

public record PrivacyPolicyResponse(bool Status, string Message, PrivacyPolicyData Data, int Code = 200);
public record PrivacyPolicyData(string Link = "https://sunsetdance.thewarriors.team/privacy-policy", string Url = "https://sunsetdance.thewarriors.team/privacy-policy");

public record SystemGenericResponse(bool Status, string Message, object? Data, int Code = 200);
