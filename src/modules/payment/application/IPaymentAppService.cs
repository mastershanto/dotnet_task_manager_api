using Payments.Domain;
using BuildingBlocks.Abstractions;

namespace Payments.Application;

public interface IPaymentAppService
{
    Task<Result<PaymentModel>> ProcessAsync(Guid userId, decimal amount, string currency);
}
