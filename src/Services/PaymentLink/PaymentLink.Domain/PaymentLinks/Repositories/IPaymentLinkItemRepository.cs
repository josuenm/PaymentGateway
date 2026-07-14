using System.Data;
using PaymentLink.Domain.PaymentLinks.Entities;

namespace PaymentLink.Domain.PaymentLinks.Repositories;

public interface IPaymentLinkItemRepository
{
    public Task<IEnumerable<PaymentLinkItem>> CreateManyAsync(
        IEnumerable<PaymentLinkItem> items, 
        IDbTransaction? transaction
    );
}