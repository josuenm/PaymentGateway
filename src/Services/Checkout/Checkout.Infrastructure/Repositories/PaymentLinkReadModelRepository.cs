using Checkout.Domain.PaymentLinkReadModels.Entities;
using Checkout.Domain.PaymentLinkReadModels.Repositories;
using Checkout.Infrastructure.Repositories.QueryResults;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace Checkout.Infrastructure.Repositories;

public class PaymentLinkReadModelRepository : IPaymentLinkReadModelRepository
{
    private readonly DapperContext _context;
    private readonly IPaymentLinkItemReadModelRepository _paymentLinkItemReadModelRepository;

    public PaymentLinkReadModelRepository(
        DapperContext dapperContext, 
        IPaymentLinkItemReadModelRepository paymentLinkItemReadModelRepository
    )
    {
        _context = dapperContext;
        _paymentLinkItemReadModelRepository = paymentLinkItemReadModelRepository;
    }

    public async Task CreateAsync(PaymentLinkReadModel readModel)
    {
        const string sql = 
@"
INSERT INTO PaymentLinkReadModels (Id, IsActive, UserId, LiveMode) VALUES (@Id, @IsActive, @UserId, @LiveMode)
";

        using var connection = _context.CreateConnection();
        var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(sql, readModel, transaction);

        if (readModel.Items.Any())
        {
            await _paymentLinkItemReadModelRepository.CreateManyAsync(readModel.Items, transaction);
        }

        transaction.Commit();
    }

    public async Task<PaymentLinkReadModel?> GetByIdAsync(string id, bool includeItems = false)
    {
        const string sql = 
@"
SELECT 
    pl.Id,
    pl.IsActive,
    pl.UserId,
    pl.LiveMode,
    
    pli.Id AS PaymentLinkItemId,
    pli.PriceId,
    pli.Quantity,
    pli.PaymentLinkId,
    pli.LiveMode AS PaymentLinkItemLiveMode
FROM PaymentLinkReadModels AS pl
LEFT JOIN PaymentLinkItemReadModels AS pli ON pl.Id = pli.PaymentLinkId
WHERE pl.Id = @Id
";
        object parameters = new { Id = id };
        var paymentLinkDictionary = new Dictionary<string, PaymentLinkReadModel>();
        
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<PaymentLinkReadModel, PaymentLinkItemQueryResult?, PaymentLinkReadModel>(
            sql,
            (paymentLink, item) =>
            {
                if (!paymentLinkDictionary.TryGetValue(paymentLink.Id, out var existingPaymentLink))
                {
                    existingPaymentLink = paymentLink;
                    existingPaymentLink.SetItems(new List<PaymentLinkItemReadModel>());
                    paymentLinkDictionary.Add(paymentLink.Id, existingPaymentLink);
                }

                if (item != null)
                {
                    var itemList = existingPaymentLink.Items.ToList();
                    itemList.Add(new PaymentLinkItemReadModel(
                        item.PaymentLinkItemId, 
                        item.PaymentLinkId, 
                        item.PriceId, 
                        item.Quantity, 
                        item.PaymentLinkItemLiveMode
                    ));
                    existingPaymentLink.SetItems(itemList);
                }
                
                return paymentLink;
            },
            parameters,
            splitOn: "PaymentLinkItemId"
        );
        
        return result.FirstOrDefault();
    }
}