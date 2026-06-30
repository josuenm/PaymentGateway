using Dapper;
using Payment.Domain.Payments.Entities;
using Payment.Domain.Payments.Repositories;
using Shared.Infrastructure.Contexts;

namespace Payment.Infrastructure.Repositories;

public class PaymentTransactionRepository : IPaymentTransactionRepository
{
    private readonly DapperContext _context;
    
    public async Task<PaymentTransactionEntity> CreateAsync(PaymentTransactionEntity paymentTransaction)
    {
        const string sql = 
@"
INSERT INTO PaymentTransactions (Id, CustomerId, Method, Status, Amount, Currency, UserId, LiveMode, CreatedAt)
VALUES (@Id, @CustomerId, @Method, @Status, @Amount, @Currency, @UserId, @ChargeId, @ChargeResponse, @LiveMode, @CreatedAt);
";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(sql, paymentTransaction);
        return paymentTransaction;
    }

    public async Task<PaymentTransactionEntity?> GetByIdAsync(string id)
    {
        const string sql = "SELECT * FROM PaymentTransactions WHERE Id = @Id;";
        
        using var connection = _context.CreateConnection();
        var paymentTransaction = await connection.QueryFirstOrDefaultAsync<PaymentTransactionEntity>(
            sql, 
            new { Id = id }
        );
        return paymentTransaction;
    }

    public async Task<bool> SetPaidByIdAsync(PaymentTransactionEntity paymentTransaction)
    {
        const string sql =
@"
UPDATE PixTransactions 
    SET Status = @Status, PaidAt = @PaidAt 
WHERE Id = @Id;
";
        var parameters = new
        {
            Id = paymentTransaction.Id,  
            PaidAt = paymentTransaction.PaidAt, 
            Status = paymentTransaction.Status
        };

        using var connection = _context.CreateConnection();
        await  connection.ExecuteAsync(sql, parameters);
        return true;
    }
}