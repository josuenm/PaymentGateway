using Checkout.Domain.ProductReadModels.Entities;
using Checkout.Domain.ProductReadModels.Repositories;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace Checkout.Infrastructure.Repositories;

public class ProductReadModelRepository : IProductReadModelRepository
{
    private readonly DapperContext _dapperContext;
    private readonly IPriceReadModelRepository _priceReadModelRepository;

    public ProductReadModelRepository(DapperContext dapperContext, IPriceReadModelRepository priceReadModelRepository)
    {
        _dapperContext = dapperContext;
        _priceReadModelRepository = priceReadModelRepository;
    }

    public async Task<ProductReadModel> CreateAsync(ProductReadModel readModel)
    {
        const string sql = "INSERT INTO ProductReadModels (Id, UserId, LiveMode) VALUES (@Id, @UserId, @LiveMode)";

        using var connection = _dapperContext.CreateConnection();
        var transaction = connection.BeginTransaction();
        
        try
        {
            await connection.ExecuteAsync(sql, readModel, transaction);
            if (readModel.Prices.Any())
            {
                await _priceReadModelRepository.CreateManyAsync(readModel.Prices, transaction);
            }

            transaction.Commit();
            return readModel;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}