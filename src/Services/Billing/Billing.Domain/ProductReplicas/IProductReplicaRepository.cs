using Billing.Domain.PriceReplicas.Enums;

namespace Billing.Domain.ProductReplicas;

public interface IProductReplicaRepository
{
    public Task<ProductReplica> CreateAsync(ProductReplica productReplica);
}