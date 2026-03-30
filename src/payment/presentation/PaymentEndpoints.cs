using App.Features.Payment.Data;
using App.Features.Payment.Domain;

namespace App.Features.Payment.Presentation;

public static class PaymentEndpoints
{
    public static void MapPayment(this WebApplication app)
    {
        app.MapPost("/payment/process", async (IPaymentService paymentService, Guid userId, decimal amount, string currency) =>
        {
            var result = await paymentService.ProcessPaymentAsync(userId, amount, currency);
            return Results.Ok(result);
        });
    }
}
