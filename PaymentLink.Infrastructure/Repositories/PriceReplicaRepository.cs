using Dapper;
using PaymentLink.Domain.PriceReplicas.Entities;
using PaymentLink.Domain.PriceReplicas.Repositories;
using Shared.Infrastructure.Contexts;

namespace PaymentLink.Infrastructure.Repositories;

public class PriceReplicaRepository : IPriceReplicaRepository
{
    private readonly DapperContext _context;
    
    public PriceReplicaRepository(DapperContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<PriceReplica>> CreateAsync(IEnumerable<PriceReplica> prices)
    {
        const string sql = "INSERT INTO PriceReplicas (Id, UserId) VALUES (@Id, @UserId)";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(sql, prices);
        
        return prices;
    }

    public async Task<IEnumerable<PriceReplica>> GetManyById(IEnumerable<string> idList)
    {
        const string sql = "SELECT * FROM PriceReplicas WHERE Id IN @IdList";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<PriceReplica>(sql, new { IdList = idList });
    }
}