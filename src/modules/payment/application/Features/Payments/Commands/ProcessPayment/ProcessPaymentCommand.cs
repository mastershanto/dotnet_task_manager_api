using BuildingBlocks.CQRS;
using Payments.Domain;

namespace Payments.Application.Features.Payments.Commands.ProcessPayment;

public record ProcessPaymentCommand(
    Guid UserId,
    decimal Amount,
    string Currency
) : ICommand<PaymentModel>;
