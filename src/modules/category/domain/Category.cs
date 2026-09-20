using System.ComponentModel.DataAnnotations;

namespace Categories.Domain;

public record CategoryModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string Description { get; init; } = string.Empty;

    [StringLength(50)]
    public string Color { get; init; } = string.Empty;

    [StringLength(50)]
    public string Icon { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
