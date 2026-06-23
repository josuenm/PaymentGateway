using System.Data;
using PaymentLink.Domain.Entities;

namespace PaymentLink.Domain.PaymentLinks.Repositories;

public interface IPaymentLinkItemRepository
{
    public Task<IEnumerable<PaymentLinkItem>> CreateManyAsync(
        IEnumerable<PaymentLinkItem> items, 
        IDbTransaction? transaction, 
        IDbConnection? connectionParam
    );
}