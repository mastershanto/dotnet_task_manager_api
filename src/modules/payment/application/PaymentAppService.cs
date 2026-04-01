using App.Features.Payment.Data;
using App.Features.Payment.Domain;
using Shared;

namespace App.Features.Payment.Application;

public class PaymentAppService : IPaymentAppService
{
    private readonly IPaymentService _paymentService;

    public PaymentAppService(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result<PaymentModel>> ProcessAsync(Guid userId, decimal amount, string currency)
    {
        if (amount <= 0) return Result<PaymentModel>.Failure("Amount must be positive");

        var payment = await _paymentService.ProcessPaymentAsync(userId, amount, currency);
        return Result<PaymentModel>.Success(payment);
    }
}
