using Catalog.Domain.Commons;
using Catalog.Domain.Prices.Entities;
using Catalog.Domain.Prices.Repositories;
using Catalog.Domain.Products.Entities;
using Catalog.Domain.Products.Repositories;
using Catalog.Infrastructure.Repositories.QueryResults;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DapperContext _context;
    private readonly IPriceRepository _priceRepository;

    public ProductRepository(DapperContext context,  IPriceRepository priceRepository)
    {
        _context = context;
        _priceRepository = priceRepository;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        const string sql = 
@"
INSERT INTO Products (Id, Name, Description, IsActive, LiveMode, UserId, Metadata, CreatedAt)
VALUES (@Id, @Name, @Description, @IsActive, @LiveMode, @UserId, @Metadata, @CreatedAt)
";
        using var connection = _context.CreateConnection();
        var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, product, transaction);
            
            if (product.Prices != null && product.Prices.Any())
            {
                await _priceRepository.CreateManyAsync(product.Prices, transaction);
            }
            
            transaction.Commit();

            return product;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<Product?> GetByIdAsync(string userId, string id)
    {
        const string sql = "SELECT * FROM Products WHERE Id = @Id AND UserId = @UserId;";
        var parameters = new
        {
            Id = id,
            UserId = userId
        };

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);
    }
    
    public async Task<Product?> GetByNameAsync(string userId, string name)
    {
        const string sql = "SELECT * FROM Products WHERE Name = @Name AND UserId = @UserId;";
        var parameters = new
        {
            Name = name,
            UserId = userId
        };
        
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);
    }

    public async Task<PagedSearchResult<Product>> GetAllPagedAsync(string userId, int page, int limit)
    {
        const string sql =
@"
SELECT
    prod.Id, 
    prod.Name, 
    prod.IsActive, 
    prod.LiveMode,
    prod.Description, 
    prod.Metadata,
    prod.UserId, 
    prod.CreatedAt, 
    prod.UpdatedAt,
    
    COUNT(*) OVER() AS Total,
    
    pri.Id AS PriceId,
    pri.Name AS PriceName,
    pri.Amount,
    pri.Currency,
    pri.Frequency,
    pri.Cycle,
    pri.UserId AS PriceUserId,
    pri.IsActive AS PriceIsActive,
    pri.LiveMode AS PriceLiveMode,
    pri.CreatedAt AS PriceCreatedAt,
    pri.UpdatedAt AS PriceUpdatedAt,
    pri.ProductId
FROM Products AS prod
LEFT JOIN Prices AS pri ON prod.Id = pri.ProductId
WHERE prod.UserId = @UserId
ORDER BY prod.CreatedAt DESC
OFFSET (@Page - 1) * @Limit ROWS
FETCH NEXT @Limit ROWS ONLY;
";

        var parameters = new
        {
            UserId = userId, 
            Page = page, 
            Limit = limit, 
        };
        
        var totalCount = 0;
        var productDictionary = new Dictionary<string, Product>();

        using var connection = _context.CreateConnection();
        var result = (await connection.QueryAsync<Product, int, PriceQueryResult?, Product>(
            sql,
            (product, total, price) =>
            {
                totalCount = total;

                if (!productDictionary.TryGetValue(product.Id, out var existingProduct))
                {
                    existingProduct = product;
                    existingProduct.SetPrices(new List<Price>());
                    productDictionary.Add(product.Id, existingProduct);
                }

                if (price != null)
                {
                    var priceList = existingProduct.Prices!.ToList();
                    priceList.Add(new Price(
                        price.PriceId, 
                        price.PriceName,
                        price.Amount, 
                        price.Currency, 
                        price.ProductId,
                        price.Frequency, 
                        price.Cycle, 
                        price.PriceUserId,
                        price.PriceLiveMode,
                        price.PriceIsActive,
                        price.PriceCreatedAt, 
                        price.PriceUpdatedAt
                    ));
                    existingProduct.SetPrices(priceList);
                }

                return existingProduct;
            },
            parameters,
            splitOn: "Total,PriceId"
        )).ToList();
        
        if (result.Count < 1)
            return new PagedSearchResult<Product>(new List<Product>(), 0);

        return new PagedSearchResult<Product>(
            result, 
            totalCount
        );
    }
    
}