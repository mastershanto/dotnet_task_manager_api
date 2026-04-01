using Payments.Domain;

namespace Payments.Data;

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
