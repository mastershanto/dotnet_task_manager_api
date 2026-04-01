using Payments.Application;
using Payments.Domain;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Security;
using Microsoft.AspNetCore.Builder;

namespace Payments.Presentation;

public static class PaymentEndpoints
{
    public static void MapPayment(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/payment")
            .WithTags("Payment")
            .RequireAuthorization(AuthPolicies.AdminOnly);

        group.MapPost("/process", async (IPaymentAppService paymentAppService, PaymentModel payment) =>
        {
            var validation = Validation.Validate(payment).ToArray();
            if (validation.Any())
                return Results.ValidationProblem(Validation.ToErrorDictionary(validation));

            var result = await paymentAppService.ProcessAsync(payment.UserId, payment.Amount, payment.Currency);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        })
        .Produces<PaymentModel>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);
    }
}
