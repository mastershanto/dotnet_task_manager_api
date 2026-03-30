using System.ComponentModel.DataAnnotations;

namespace App.Features.Payment.Domain;

public record PaymentModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; init; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; init; }

    [Required]
    [StringLength(10)]
    public string Currency { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
