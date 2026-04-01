using System.ComponentModel.DataAnnotations;

namespace App.Features.Product.Domain;

public record ProductModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; init; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; init; }

    [Required]
    public string Category { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
