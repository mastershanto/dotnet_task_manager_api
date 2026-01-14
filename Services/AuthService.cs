using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Services;

public sealed class AuthService : IAuthService
{
    private readonly TodoContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        TodoContext context,
        IJwtTokenService jwtTokenService,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var usernameTaken = await _context.Users.AnyAsync(u => u.Username == dto.Username);
        if (usernameTaken)
            throw new InvalidOperationException("Username is already taken");

        var emailTaken = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailTaken)
            throw new InvalidOperationException("Email is already registered");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            FullName = dto.FullName,
            Roles = new[] { "User" },
            Status = UserStatus.Active,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var (token, expiresAt) = _jwtTokenService.CreateAccessToken(user);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password");

        if (user.Status != UserStatus.Active)
            throw new UnauthorizedAccessException("User is not active");

        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verify == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid username or password");

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var (token, expiresAt) = _jwtTokenService.CreateAccessToken(user);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Token = token,
            ExpiresAt = expiresAt
        };
    }
}
