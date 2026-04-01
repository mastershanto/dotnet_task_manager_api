namespace Auth.Domain;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(AuthenticationRequest request);
}
