using App.Features.Payment.Domain;
using Shared;

namespace App.Features.Payment.Application;

public interface IPaymentAppService
{
    Task<Result<PaymentModel>> ProcessAsync(Guid userId, decimal amount, string currency);
}
