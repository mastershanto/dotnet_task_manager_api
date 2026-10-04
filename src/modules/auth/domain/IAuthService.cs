namespace Auth.Domain;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(AuthenticationRequest request);
    Task<RegisterResult> RegisterAsync(RegisterRequest request);
    Task<OtpResult> VerifyOtpAsync(OtpRequest request);
    Task<RegisterResult> ResendOtpAsync(PasswordResetRequest request);
    Task<PasswordResetResult> PasswordResetRequestAsync(PasswordResetRequest request);
    Task<PasswordResetResult> PasswordResetResendAsync(PasswordResetRequest request);
    Task<PasswordResetResult> PasswordResetVerifyAsync(PasswordResetVerifyRequest request);
    Task<PasswordResetResult> PasswordResetConfirmAsync(PasswordResetConfirmRequest request);
}
