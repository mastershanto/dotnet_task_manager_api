using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly TodoContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(TodoContext context, IPasswordHasher passwordHasher, ITokenService tokenService, ILogger<AuthController> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto dto)
    {
        var usernameTaken = await _context.Users.AnyAsync(u => u.Username == dto.Username);
        if (usernameTaken)
            return Conflict(new ApiResponse<object> { Success = false, Message = "Username already exists" });

        var emailTaken = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailTaken)
            return Conflict(new ApiResponse<object> { Success = false, Message = "Email already exists" });

        var user = new User
        {
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim(),
            FullName = string.IsNullOrWhiteSpace(dto.FullName) ? null : dto.FullName.Trim(),
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            Roles = new[] { "User" },
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var (token, expiresAt) = _tokenService.CreateToken(user);

        _logger.LogInformation("User {UserId} registered", user.Id);

        return Ok(new ApiResponse<AuthResponseDto>
        {
            Success = true,
            Message = "Registration successful",
            Data = new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
                ExpiresAt = expiresAt
            }
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Username);
        if (user == null)
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid credentials" });

        if (user.Status != UserStatus.Active)
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "User is not active" });

        var ok = _passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);
        if (!ok)
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid credentials" });

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var (token, expiresAt) = _tokenService.CreateToken(user);

        return Ok(new ApiResponse<AuthResponseDto>
        {
            Success = true,
            Message = "Login successful",
            Data = new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
                ExpiresAt = expiresAt
            }
        });
    }
}
