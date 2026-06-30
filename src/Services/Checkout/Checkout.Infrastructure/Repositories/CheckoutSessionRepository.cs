using Checkout.Domain.Checkouts.Entities;
using Checkout.Domain.Checkouts.Repositories;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace Checkout.Infrastructure.Repositories;

public class CheckoutSessionRepository : ICheckoutSessionRepository
{
    private readonly DapperContext _context;

    public CheckoutSessionRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<CheckoutSession> CreateAsync(CheckoutSession session)
    {
        const string sql = 
@"
INSERT INTO CheckoutSessions 
    (Id, SessionId, CustomerId, PaymentId, PaymentLinkId, Amount, Currency, UserId, PaymentMethod, CreatedAt)
VALUES 
    (@Id, @SessionId, @CustomerId, @PaymentId, PaymentMethod, @Amount, @Currency, @UserId, @PaymentMethod, @CreatedAt)
";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(sql, session);
        return session;
    }

    public async Task<CheckoutSession?> GetById(string sessionId)
    {
        const string sql = "SELECT * FROM CheckoutSessions WHERE Id = @Id";

        using var connection = _context.CreateConnection();
        var session = await connection.QueryFirstOrDefaultAsync<CheckoutSession>(sql, new { Id = sessionId });
        return session;
    }
}