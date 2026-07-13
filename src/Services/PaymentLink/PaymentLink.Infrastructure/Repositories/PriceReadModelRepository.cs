using System.Data;
using PaymentLink.Domain.ProductReadModels.Entities;
using PaymentLink.Domain.ProductReadModels.Repositories;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace PaymentLink.Infrastructure.Repositories;

public class PriceReadModelRepository : IPriceReadModelRepository
{
    private readonly DapperContext _dapperContext;

    public PriceReadModelRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }
    
    public async Task<IEnumerable<PriceReadModel>> CreateManyAsync(
        IEnumerable<PriceReadModel> prices, 
        IDbTransaction? transaction = null
    )
    {
        const string sql = 
@"
INSERT INTO PriceReadModels (Id, Name, Frequency, Cycle, ProductId, Amount, Currency, UserId, LiveMode) 
VALUES (@Id, @Name, @Frequency, @Cycle, @ProductId, @Amount, @Currency, @UserId, @LiveMode);
";

        if (transaction != null)
        {
            await transaction.Connection!.ExecuteAsync(sql, prices, transaction);
        }
        else
        {
            using var connection = _dapperContext.CreateConnection();
            await connection.ExecuteAsync(sql, prices);
        }

        return prices;
    }

    public async Task<IEnumerable<PriceReadModel>> GetManyByIdAsync(IEnumerable<string> idList)
    {
        const string sql = "SELECT * FROM PriceReadModels WHERE Id IN @IdList";
        var parameters = new { IdList = idList };

        using var connection = _dapperContext.CreateConnection();
        return await connection.QueryAsync<PriceReadModel>(sql, parameters);
    }
}