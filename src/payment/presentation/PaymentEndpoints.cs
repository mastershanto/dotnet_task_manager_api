using App.Features.Payment.Application;
using App.Features.Payment.Domain;

namespace App.Features.Payment.Presentation;

public static class PaymentEndpoints
{
    public static void MapPayment(this WebApplication app)
    {
        app.MapPost("/payment/process", async (IPaymentAppService paymentAppService, Guid userId, decimal amount, string currency) =>
        {
            var result = await paymentAppService.ProcessAsync(userId, amount, currency);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        });
    }
}
