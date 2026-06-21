using System.Data;
using Billing.Domain.PriceReplicas;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace Billing.Infrastructure.Repositories;

public class PriceReplicaRepository : IPriceReplicaRepository
{
    private readonly DapperContext _context;

    public PriceReplicaRepository(DapperContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<PriceReplica>> CreateManyAsync(
        IEnumerable<PriceReplica> prices, 
        IDbTransaction? transaction = null, 
        IDbConnection? connectionParam = null
    )
    {
        const string sql = 
@"
INSERT INTO PricesReplicas
    (Id, Name, AmountInCents, Currency, Frequency, ProductReplicaId, Cycle, IsActive, LiveMode)
VALUES (@Id, @Name, @AmountInCents, @Currency, @Frequency, @ProductReplicaId, @Cycle, @IsActive, @LiveMode);
";
        
        var connection = connectionParam ?? _context.CreateConnection();
            
        await connection.ExecuteAsync(sql, prices, transaction);
            
        return prices;
    }
}