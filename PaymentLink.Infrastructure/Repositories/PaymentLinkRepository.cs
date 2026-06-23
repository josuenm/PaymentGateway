using System.Text.Json;
using Dapper;
using PaymentLink.Domain.Entities;
using PaymentLink.Domain.PaymentLinks.Repositories;
using Shared.Infrastructure.Contexts;

namespace PaymentLink.Infrastructure.Repositories;

public class PaymentLinkRepository : IPaymentLinkRepository
{
    private readonly DapperContext _context;
    private readonly IPaymentLinkItemRepository _paymentLinkItemRepository;
    
    public PaymentLinkRepository(DapperContext context, IPaymentLinkItemRepository paymentLinkItemRepository)
    {
        _context = context;
        _paymentLinkItemRepository = paymentLinkItemRepository;
    }

    public async Task<PaymentLinkEntity> CreateAsync(PaymentLinkEntity paymentLink)
    {
        const string sql =
@"
INSERT INTO PaymentLinks (Id, Methods, UserId, LiveMode, IsActive, CreatedAt)
VALUES (@Id, @Methods, @UserId, @LiveMode, @IsActive, @CreatedAt)
";

        using var connection = _context.CreateConnection();
        var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(
            sql, 
            new
            {
                Id = paymentLink.Id, 
                Methods = JsonSerializer.Serialize(
                    paymentLink.Methods.Select(m => m.ToString()).ToList()
                ), 
                UserId = paymentLink.UserId, 
                LiveMode = paymentLink.LiveMode, 
                IsActive = paymentLink.IsActive, 
                CreatedAt = paymentLink.CreatedAt 
            }, 
            transaction
        );
        if (paymentLink.Items.Any())
        {
            await _paymentLinkItemRepository.CreateManyAsync(paymentLink.Items, transaction, connection);
        }

        transaction.Commit();
        
        return paymentLink;
    }

    public async Task<PaymentLinkEntity?> GetByIdAsync(string id, bool includeItems = false)
    {
        const string sql = 
@"
SELECT * FROM PaymentLinks WHERE Id = @Id;
    
IF (@IncludeItems = 1)
BEGIN
    SELECT * FROM PaymentLinkItems WHERE PaymentLinkId = @Id;    
END
";

        var parameters = new { Id = id, IncludeItems = includeItems ? 1 : 9 };
        
        using var connection = _context.CreateConnection();
        
        await using var multi = await connection.QueryMultipleAsync(sql, parameters);

        var paymentLink = multi.ReadFirstOrDefault<PaymentLinkEntity>();

        if (paymentLink != null)
        {
            var items = await multi.ReadAsync<PaymentLinkItem>();
            paymentLink.SetItems(items);
        }

        return paymentLink;
    }
}