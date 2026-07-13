using Checkout.Domain.PaymentLinkReadModels.Entities;

namespace Checkout.Domain.PaymentLinkReadModels.Repositories;

public interface IPaymentLinkReadModelRepository
{
    public Task CreateAsync(PaymentLinkReadModel readModel);
    public Task<PaymentLinkReadModel?> GetByIdAsync(string id, bool includeItems = false);
}