using System.Data;
using Checkout.Domain.PaymentLinkReadModels.Entities;
using Checkout.Domain.PaymentLinkReadModels.Repositories;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace Checkout.Infrastructure.Repositories;

public class PaymentLinkItemReadModelRepository : IPaymentLinkItemReadModelRepository
{
    private readonly DapperContext _context;

    public PaymentLinkItemReadModelRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task CreateManyAsync(
        IEnumerable<PaymentLinkItemReadModel> readModel,
        IDbTransaction? transaction = null
    )
    {
        const string sql = 
@"
INSERT INTO PaymentLinkItemReadModel (Id, PaymentLinkId, PriceId, Quantity, LiveMode)
VALUES (@Id, @PaymentLinkId, @PriceId, @Quantity, @LiveMode);
";

        using var connection = transaction?.Connection ?? _context.CreateConnection();
        await connection.ExecuteAsync(sql, readModel, transaction);
    }
}