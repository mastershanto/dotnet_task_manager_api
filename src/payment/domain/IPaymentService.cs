namespace App.Features.Payment.Domain;

public interface IPaymentService
{
    Task<PaymentModel> ProcessPaymentAsync(Guid userId, decimal amount, string currency);
}
