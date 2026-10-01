using Auth.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.Login;

/// <summary>
/// লগইন হ্যান্ডলার (Clean Architecture CQRS Handler):
/// ১. ইউজার অস্তিত্ব যাচাই করে।
/// ২. পাসওয়ার্ড হ্যাশ মিলিয়ে দেখে (Secure PBKDF2)।
/// ৩. ইমেইল ভেরিফাইড কিনা তা নিশ্চিত করে (অপ্রমাণিত একাউন্টে ওটিপি ভেরিফাই করতে অনুরোধ করে)।
/// ৪. সফল হলে JWT টোকেন ও প্রোফাইল ডাটা প্রদান করে।
/// </summary>
public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
{
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasherService _hasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IAuthRepository authRepository,
        IPasswordHasherService hasher,
        ITokenService tokenService)
    {
        _authRepository = authRepository;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result<AuthResponse>.Failure("Invalid email or password.");
        }

        var isPasswordValid = _hasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<AuthResponse>.Failure("Invalid email or password.");
        }

        if (!user.IsEmailVerified)
        {
            return Result<AuthResponse>.Failure("Your email is not verified yet. Please verify using the OTP sent to your email.");
        }

        var token = _tokenService.GenerateToken(user);

        var response = new AuthResponse(
            Success: true,
            Message: "Login successful.",
            Token: token,
            UserId: user.Id,
            Name: user.Name,
            Email: user.Email,
            Role: user.Role
        );

        return Result<AuthResponse>.Success(response);
    }
}
