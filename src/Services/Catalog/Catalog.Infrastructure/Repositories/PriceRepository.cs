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

    public async Task<IEnumerable<Price>> GetManyByIdAsync(
        IEnumerable<string> idList,
        bool? priceActive = true, 
        bool? productActive = true
    )
    {
        const string sql = 
@"
SELECT 
    pri.Id AS Id, 
    pri.Name AS Name, 
    pri.Amount AS Amount, 
    pri.Currency AS Currency, 
    pri.ProductId AS ProductId, 
    pri.IsActive  AS IsActive, 
    pri.Frequency  AS Frequency, 
    pri.UserId AS USerId,
    pri.Cycle  AS Cycle
FROM Prices AS pri
    INNER JOIN Products as pro ON pro.Id = pri.ProductId
WHERE pri.Id IN @IdList 
    AND pri.IsActive = @PriceActive
    AND pro.IsActive = @ProductActive;
";
        
        var parameters = new
        {
            @PriceActive = priceActive == true ? 1 : 0,
            @productActive = productActive == true ? 1 : 0,
            IdList = idList,
        };

        var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<Price>(sql, parameters);
        
        return result;
    }
    
    public async Task<IEnumerable<Price>> CreateManyAsync(
        IEnumerable<Price> prices, 
        IDbTransaction? transaction = null 
    )
    {
        const string sql = 
            @"
INSERT INTO Prices (Id, Name, Amount, Frequency, Cycle, Currency, ProductId, UserId, LiveMode, IsActive, CreatedAt)
VALUES (@Id, @Name, @Amount, @Frequency, @Cycle, @Currency, @ProductId, @UserId, @LiveMode, @IsActive, @CreatedAt)
";
        
        var connection = transaction?.Connection ?? _context.CreateConnection();
            
        await connection.ExecuteAsync(sql, prices, transaction);
            
        return prices;
    }
    
    public async Task<Price> CreateAsync(Price price)
    {
        const string sql = 
@"
INSERT INTO Prices (Id, Name, Amount, Frequency, Cycle, Currency, ProductId, UserId, LiveMode, IsActive, CreatedAt)
VALUES (@Id, @Name, @Amount, @Frequency, @Cycle, @Currency, @ProductId, @UserId, @LiveMode, @IsActive, @CreatedAt)
";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(sql, price);

        return price;
    }

    public async Task<PagedSearchResult<Price>> GetAllPagedAsync(string userId, int page, int limit)
    {
        const string sql = 
@"
SELECT 
    Id,
    Name, 
    Amount, 
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
        
        var parameters = new
        {
            UserId = userId,
            Page = page,
            Limit = limit
        };
            
        var totalCount = 0;

        using var connection = _context.CreateConnection();
        
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