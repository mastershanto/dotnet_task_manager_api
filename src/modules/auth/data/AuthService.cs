using Auth.Domain;

namespace Auth.Data;

/// <summary>
/// প্রমাণীকরণ সার্ভিস ইমপ্লিমেন্টেশন (Facade for Legacy / Direct Calls):
/// ডাটাবেসের ইউজার ও পাসওয়ার্ড ভেরিফাই করে JWT টোকেন প্রদান করে।
/// </summary>
public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasherService _hasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IAuthRepository authRepository,
        IPasswordHasherService hasher,
        ITokenService tokenService)
    {
        _authRepository = authRepository;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResult> AuthenticateAsync(AuthenticationRequest request)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email);
        if (user is null)
        {
            return new AuthResult(false, "Invalid email or password", null);
        }

        var isPasswordValid = _hasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return new AuthResult(false, "Invalid email or password", null);
        }

        if (!user.IsEmailVerified)
        {
            return new AuthResult(false, "Please verify your email via OTP first.", null);
        }

        var token = _tokenService.GenerateToken(user);
        return new AuthResult(true, "Authentication successful", token);
    }
}
