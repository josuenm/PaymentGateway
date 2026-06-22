using Billing.Domain.PriceReplicas;
using Billing.Domain.ProductReplicas;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace Billing.Infrastructure.Repositories;

public class ProductReplicaRepository : IProductReplicaRepository
{
    private readonly DapperContext _context;
    private readonly IPriceReplicaRepository _priceReplicaRepository;

    public ProductReplicaRepository(DapperContext dapperContext, IPriceReplicaRepository priceReplicaRepository)
    {
        _context = dapperContext;
        _priceReplicaRepository = priceReplicaRepository;
    }
    
    public async Task<ProductReplica> CreateAsync(ProductReplica productReplica)
    {
        const string sql = 
@"
INSERT INTO ProductReplicas 
    (Id, Name, Description, Metadata, UserId, IsActive, LiveMode)
VALUES (@Id, @Name, @Description, @Metadata, @UserId, @IsActive, @LiveMode);
";
        
        using var connection = _context.CreateConnection();
        var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(sql, productReplica, transaction);

        if (productReplica.Prices != null && productReplica.Prices.Any())
        {
            await _priceReplicaRepository.CreateManyAsync(productReplica.Prices, transaction, connection);
        }
            
        transaction.Commit();
        return productReplica;
    }
}