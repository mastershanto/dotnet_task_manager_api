using App.Features.Payment.Domain;

namespace App.Features.Payment.Data;

public class PaymentService : IPaymentService
{
    public Task<PaymentModel> ProcessPaymentAsync(Guid userId, decimal amount, string currency)
    {
        var payment = new PaymentModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount,
            Currency = currency,
            CreatedAt = DateTime.UtcNow
        };

        return Task.FromResult(payment);
    }
}
