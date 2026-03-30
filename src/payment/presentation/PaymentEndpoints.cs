using App.Features.Payment.Application;
using App.Features.Payment.Domain;
using Shared;

namespace App.Features.Payment.Presentation;

public static class PaymentEndpoints
{
    public static void MapPayment(this WebApplication app)
    {
        app.MapPost("/payment/process", async (IPaymentAppService paymentAppService, PaymentModel payment) =>
        {
            var validation = Validation.Validate(payment).ToArray();
            if (validation.Any())
                return Results.BadRequest(validation.Select(x => x.ErrorMessage));

            var result = await paymentAppService.ProcessAsync(payment.UserId, payment.Amount, payment.Currency);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        })
        .WithTags("Payment");
    }
}
