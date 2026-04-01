using Payments.Domain;
using Npgsql;

namespace Payments.Data;

public class PostgresPaymentService : IPaymentService
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresPaymentService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<PaymentModel> ProcessPaymentAsync(Guid userId, decimal amount, string currency)
    {
        var payment = new PaymentModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount,
            Currency = currency,
            CreatedAt = DateTime.UtcNow
        };

        const string sql = """
            INSERT INTO payments (id, user_id, amount, currency, created_at)
            VALUES (@id, @userId, @amount, @currency, @createdAt);
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", payment.Id);
        command.Parameters.AddWithValue("userId", payment.UserId);
        command.Parameters.AddWithValue("amount", payment.Amount);
        command.Parameters.AddWithValue("currency", payment.Currency);
        command.Parameters.AddWithValue("createdAt", payment.CreatedAt);

        await command.ExecuteNonQueryAsync();
        return payment;
    }
}
