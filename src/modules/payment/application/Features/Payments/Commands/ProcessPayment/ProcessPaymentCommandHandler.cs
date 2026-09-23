using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Payments.Domain;
using Users.Domain;

namespace Payments.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand, PaymentModel>
{
    private readonly IPaymentService _paymentService;
    private readonly IUserRepository _userRepository;

    public ProcessPaymentCommandHandler(IPaymentService paymentService, IUserRepository userRepository)
    {
        _paymentService = paymentService;
        _userRepository = userRepository;
    }

    public async Task<Result<PaymentModel>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
        {
            return Result<PaymentModel>.Failure("Amount must be positive");
        }

        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
        {
            return Result<PaymentModel>.Failure("User not found");
        }

        var payment = await _paymentService.ProcessPaymentAsync(request.UserId, request.Amount, request.Currency);
        return Result<PaymentModel>.Success(payment);
    }
}
