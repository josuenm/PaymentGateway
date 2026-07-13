using System.Text.Json;
using Customer.Domain.Commons;
using Dapper;
using PaymentLink.Domain.Entities;
using PaymentLink.Domain.PaymentLinks.Repositories;
using PaymentLink.Infrastructure.Repositories.QueryResults;
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
            await _paymentLinkItemRepository.CreateManyAsync(paymentLink.Items, transaction);
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

    public async Task<PagedSearchResult<PaymentLinkEntity>> GetAllPagedAsync(string userId, int page, int limit)
    {
        const string sql = 
@"
SELECT 
    pl.Id, 
    pl.UserId, 
    pl.Methods, 
    pl.LiveMode, 
    pl.IsActive, 
    pl.CreatedAt, 
    pl.UpdatedAt,
    
    COUNT(*) OVER() AS Total,
    
    pli.Id AS PaymentLinkItemId,
    pli.PriceId,
    pli.UserId AS PaymentLinkItemUserId,
    pli.LiveMode AS PaymentLinkItemLiveMode,
    pli.Quantity,
    pli.PaymentLinkId,
    pli.CreatedAt AS PaymentLinkItemCreatedAt,
    pli.UpdatedAt AS PaymentLinkItemUpdatedAt
FROM PaymentLinks AS pl
LEFT JOIN PaymentLinkItems AS pli ON pl.Id = pli.PaymentLinkId
WHERE pl.UserId = @UserId
ORDER BY pl.CreatedAt DESC
OFFSET (@Page - 1) * @Limit ROWS
FETCH NEXT @Limit ROWS ONLY;
";

        var parameters = new
        {
            UserId = userId, 
            Page = page, 
            Limit = limit, 
        };

        var totalCount = 0;
        var paymentLinkDictionary = new Dictionary<string, PaymentLinkEntity>();
        
        using var connection = _context.CreateConnection();

        var result = (await connection.QueryAsync<PaymentLinkEntity, int, PaymentLinkItemQueryResult?, PaymentLinkEntity>(
            sql,
            (paymentLink, total, item) =>
            {
                totalCount = total;

                if (!paymentLinkDictionary.TryGetValue(paymentLink.Id, out var existingItem))
                {
                    existingItem = paymentLink;
                    existingItem.SetItems(new List<PaymentLinkItem>());
                    paymentLinkDictionary.Add(paymentLink.Id, existingItem);
                }

                if (item != null)
                {
                    var itemList = existingItem.Items.ToList();
                    itemList.Add(new PaymentLinkItem(
                        item.PaymentLinkItemId,
                        item.PriceId, 
                        item.PaymentLinkItemUserId, 
                        item.PaymentLinkId, 
                        item.Quantity, 
                        item.PaymentLinkItemLiveMode,
                        item.PaymentLinkItemCreatedAt,
                        item.PaymentLinkItemUpdatedAt
                    ));
                    existingItem.SetItems(itemList);
                }
                
                return existingItem;
            },
            parameters,
            splitOn: "Total,PaymentLinkItemId"
        )).Distinct().ToList();

        if (result.Count < 1)
            return new PagedSearchResult<PaymentLinkEntity>(new List<PaymentLinkEntity>(), 0);

        return new PagedSearchResult<PaymentLinkEntity>(result, totalCount);
    }
}