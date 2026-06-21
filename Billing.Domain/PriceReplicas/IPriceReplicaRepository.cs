using System.Data;

namespace Billing.Domain.PriceReplicas;

public interface IPriceReplicaRepository
{
    public Task<IEnumerable<PriceReplica>> CreateManyAsync(
        IEnumerable<PriceReplica> prices, 
        IDbTransaction? transaction = null, 
        IDbConnection? connectionParam = null
    );
}