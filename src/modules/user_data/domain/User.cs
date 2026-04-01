using System.ComponentModel.DataAnnotations;

namespace App.Features.User.Domain;

public record UserModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
