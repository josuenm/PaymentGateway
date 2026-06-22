using System.Data;
using Catalog.Domain.Commons;
using Catalog.Domain.Prices.Entities;
using Catalog.Domain.Prices.Repositories;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace Catalog.Infrastructure.Repositories;

public class PriceRepository : IPriceRepository
{
    private readonly DapperContext _context;
    
    public PriceRepository(DapperContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Price>> CreateManyAsync(
        IEnumerable<Price> prices, 
        IDbTransaction? transaction, 
        IDbConnection? connectionParam = null
    )
    {
        const string sql = 
            @"
INSERT INTO Prices (Id, Name, AmountInCents, Frequency, Cycle, Currency, ProductId, UserId, LiveMode, IsActive, CreatedAt)
VALUES (@Id, @Name, @AmountInCents, @Frequency, @Cycle, @Currency, @ProductId, @UserId, @LiveMode, @IsActive, @CreatedAt)
";
        
        try
        {
            var connection = connectionParam ?? _context.CreateConnection();
            
            await connection.ExecuteAsync(sql, prices, transaction);
            
            return prices;
        }
        catch (Exception e)
        {
            throw;
        }
    }
    
    public async Task<Price> CreateAsync(Price price)
    {
        const string sql = 
@"
INSERT INTO Prices (Id, Name, AmountInCents, Frequency, Cycle, Currency, ProductId, UserId, LiveMode, IsActive, CreatedAt)
VALUES (@Id, @Name, @AmountInCents, @Frequency, @Cycle, @Currency, @ProductId, @UserId, @LiveMode, @IsActive, @CreatedAt)
";
        
        try
        {
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, price);
            }
            return price;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<PagedSearchResult<Price>> GetAllPagedAsync(string userId, int page, int limit)
    {
        const string sql = 
@"
SELECT 
    Id,
    Name, 
    AmountInCents, 
    Currency, 
    Frequency, 
    Cycle, 
    ProductId,
    UserId, 
    LiveMode, 
    IsActive, 
    CreatedAt, 
    COUNT(*) OVER() AS Total
FROM Prices
WHERE UserId = @UserId
ORDER BY
    CreatedAt DESC
OFFSET (@Page - 1) * @Limit ROWS
FETCH NEXT @Limit ROWS ONLY;
";
        
        try
        {
            var parameters = new
            {
                UserId = userId,
                Page = page,
                Limit = limit
            };
            
            var totalCount = 0; 
            
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Price, int, Price>(
                    sql,
                    (price, total) =>
                    {
                        totalCount = total;
                        return price;
                    }, 
                    parameters, 
                    splitOn: "Total"
                );

                if (!result.Any())
                    return new PagedSearchResult<Price>(new List<Price>(), totalCount);
                
                return new PagedSearchResult<Price>(result.ToList(), totalCount);
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
}