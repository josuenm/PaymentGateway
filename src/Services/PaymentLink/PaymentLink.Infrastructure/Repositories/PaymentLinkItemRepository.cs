using System.Data;
using Dapper;
using PaymentLink.Domain.Entities;
using PaymentLink.Domain.PaymentLinks.Repositories;
using Shared.Infrastructure.Contexts;

namespace PaymentLink.Infrastructure.Repositories;

public class PaymentLinkItemRepository : IPaymentLinkItemRepository
{
    private readonly DapperContext _context;
    
    public PaymentLinkItemRepository(DapperContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<PaymentLinkItem>> CreateManyAsync(
        IEnumerable<PaymentLinkItem> items,
        IDbTransaction? transaction
    )
    {
        const string sql = 
@"
INSERT INTO PaymentLinkItems (Id, UserId, PaymentLinkId, PriceId, Quantity, LiveMode, CreatedAt)
VALUES (@Id, @UserId, @PaymentLinkId, @PriceId, @Quantity, @LiveMode, @CreatedAt)
";

        var connection = transaction?.Connection ?? _context.CreateConnection();
        
        await connection.ExecuteAsync(sql, items, transaction);
        
        return items;
    }
}