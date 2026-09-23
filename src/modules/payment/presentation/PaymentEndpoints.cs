using Payments.Application.Features.Payments.Commands.ProcessPayment;
using Payments.Domain;
using BuildingBlocks.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Payments.Presentation;

public static class PaymentEndpoints
{
    public static void MapPayment(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/payment")
            .WithTags("Payment")
            .RequireAuthorization(AuthPolicies.AdminOnly);

        // POST /payment/process (COMMAND)
        group.MapPost("/process", async (ISender sender, PaymentModel payment, CancellationToken cancellationToken) =>
        {
            var command = new ProcessPaymentCommand(payment.UserId, payment.Amount, payment.Currency);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSuccess 
                ? Results.Ok(result.Value) 
                : Results.BadRequest(new { errors = result.Errors });
        })
        .Produces<PaymentModel>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);
    }
}
