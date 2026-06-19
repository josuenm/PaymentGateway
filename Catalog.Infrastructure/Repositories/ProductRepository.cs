using Catalog.Domain.Commons;
using Catalog.Domain.Prices.Repositories;
using Catalog.Domain.Products.Entities;
using Catalog.Domain.Products.Repositories;
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

        try
        {
            using var connection = _context.CreateConnection();
            
            var transaction = connection.BeginTransaction();
            
            await connection.ExecuteAsync(sql, product, transaction);
            
            if (product.Prices != null && product.Prices.Any())
            {
                await _priceRepository.CreateManyAsync(product.Prices, transaction, connection);
            }
            
            transaction.Commit();

            return product;
        }
        catch (Exception e)
        {
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
        
        try
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
    
    public async Task<Product?> GetByNameAsync(string userId, string name)
    {
        const string sql = "SELECT * FROM Products WHERE Name = @Name AND UserId = @UserId;";
        var parameters = new
        {
            Name = name,
            UserId = userId
        };
        
        try
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<PagedSearchResult<Product>> GetAllPagedAsync(string userId, int page, int limit)
    {
        const string sql =
@"
SELECT
    Id, 
    Name, 
    IsActive, 
    LiveMode,
    Description, 
    Metadata,
    UserId, 
    CreatedAt, 
    UpdatedAt,
    COUNT(*) OVER() AS Total
FROM Products
WHERE UserId = @UserId
ORDER BY CreatedAt DESC
OFFSET (@Page - 1) * @Limit ROWS
FETCH NEXT @Limit ROWS ONLY;
";

        var parameters = new
        {
            UserId = userId, 
            Page = page, 
            Limit = limit, 
        };
        
        try
        {
            var totalCount = 0;
            
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Product, int, Product>(
                    sql,
                    (product, total) =>
                    {
                        totalCount = total;
                        return product;
                    },
                    parameters,
                    splitOn: "Total"
                );
                
                if (!result.Any())
                    return new PagedSearchResult<Product>(new List<Product>(), 0);

                var products = result.ToList();

                return new PagedSearchResult<Product>(
                    products, 
                    totalCount
                );
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
    
}