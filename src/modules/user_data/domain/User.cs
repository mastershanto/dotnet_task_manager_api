using System.ComponentModel.DataAnnotations;

namespace Users.Domain;

/// <summary>
/// User ডোমেন মডেল (User Domain Entity):
/// Clean Architecture অনুযায়ী এটি সম্পূর্ণ ফ্রেমওয়ার্ক-স্বাধীন পিওর C# অবজেক্ট।
/// </summary>
public record UserModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    public string PasswordHash { get; init; } = string.Empty;

    public string Role { get; init; } = "user";

    public bool IsEmailVerified { get; init; } = false;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; init; }
}
