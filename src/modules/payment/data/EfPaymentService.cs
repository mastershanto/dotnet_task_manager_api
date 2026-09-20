using BuildingBlocks.Persistence;
using Payments.Domain;

namespace Payments.Data;

public class EfPaymentService : IPaymentService
{
    private readonly AppDbContext _context;

    public EfPaymentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentModel> ProcessPaymentAsync(Guid userId, decimal amount, string currency)
    {
        var payment = new PaymentModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount,
            Currency = currency,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
        return payment;
    }
}
