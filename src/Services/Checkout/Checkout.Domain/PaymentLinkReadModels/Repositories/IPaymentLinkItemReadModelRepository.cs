using System.Data;
using Checkout.Domain.PaymentLinkReadModels.Entities;

namespace Checkout.Domain.PaymentLinkReadModels.Repositories;

public interface IPaymentLinkItemReadModelRepository
{
    public Task CreateManyAsync(
        IEnumerable<PaymentLinkItemReadModel> readModel,
        IDbTransaction? transaction = null
    );
}