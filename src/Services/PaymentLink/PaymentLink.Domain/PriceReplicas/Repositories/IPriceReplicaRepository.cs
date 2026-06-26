using PaymentLink.Domain.PriceReplicas.Entities;

namespace PaymentLink.Domain.PriceReplicas.Repositories;

public interface IPriceReplicaRepository
{
    public Task<IEnumerable<PriceReplica>> CreateAsync(IEnumerable<PriceReplica> prices);
    public Task<IEnumerable<PriceReplica>> GetManyById(IEnumerable<string> idList);
}