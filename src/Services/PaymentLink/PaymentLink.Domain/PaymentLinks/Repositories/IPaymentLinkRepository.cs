using Customer.Domain.Commons;
using PaymentLink.Domain.Entities;

namespace PaymentLink.Domain.PaymentLinks.Repositories;

public interface IPaymentLinkRepository
{
    public Task<PaymentLinkEntity> CreateAsync(PaymentLinkEntity paymentLink);
    public Task<PaymentLinkEntity?> GetByIdAsync(string id, bool includeItems = false);
    public Task<PagedSearchResult<PaymentLinkEntity>> GetAllPagedAsync(string userId, int page, int limit);
}