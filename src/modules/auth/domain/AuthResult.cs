namespace Auth.Domain;

public record AuthResult(bool Success, string Message, string? Token);
