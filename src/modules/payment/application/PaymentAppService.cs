using Payments.Domain;
using Users.Domain;
using BuildingBlocks.Abstractions;

namespace Payments.Application;

public class PaymentAppService : IPaymentAppService
{
    private readonly IPaymentService _paymentService;
    private readonly IUserRepository _userRepository;

    public PaymentAppService(IPaymentService paymentService, IUserRepository userRepository)
    {
        _paymentService = paymentService;
        _userRepository = userRepository;
    }

    public async Task<Result<PaymentModel>> ProcessAsync(Guid userId, decimal amount, string currency)
    {
        if (amount <= 0) return Result<PaymentModel>.Failure("Amount must be positive");

        var user = await _userRepository.GetAsync(userId);
        if (user is null) return Result<PaymentModel>.Failure("User not found");

        var payment = await _paymentService.ProcessPaymentAsync(userId, amount, currency);
        return Result<PaymentModel>.Success(payment);
    }
}
