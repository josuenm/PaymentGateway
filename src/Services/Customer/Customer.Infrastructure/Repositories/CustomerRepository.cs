using Customer.Domain.Commons;
using Customer.Domain.Customers.Entities;
using Customer.Domain.Customers.Repositories;
using Shared.Infrastructure.Contexts;
using Dapper;

namespace Customer.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DapperContext _context;

    public CustomerRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<CustomerEntity> CreateAsync(CustomerEntity customer)
    {
        const string sql = 
@"
INSERT INTO Customers (Id, Email, Name, TaxId, LiveMode, UserId, CreatedAt) 
VALUES (@Id, @Email, @Name, @TaxId, 0, @UserId, @CreatedAt)
";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(sql, customer);
        return customer;
    }

    public async Task<CustomerEntity?> GetByEmailAsync(string userId, string email)
    {
        const string sql = "SELECT * FROM Customers WHERE Email = @Email AND UserId = @UserId;";
        var parameters = new { Email = email, UserId = userId };
        
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CustomerEntity>(sql, parameters);   
    }

    public async Task<CustomerEntity?> GetByIdAsync(string userId, string id)
    {
        const string sql = "SELECT * FROM Customers WHERE Id = @Id AND UserId = @UserId;";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CustomerEntity>(sql, new { Id = id, UserId = userId });
    }

    public async Task<PagedSearchResult<CustomerEntity>> GetAllAsync(string userId, int page, int limit)
    {
        const string sql = 
@"
SELECT
    Id,
    Name,
    Email,
    TaxId,
    LiveMode,
    UserId,
    CreatedAt,
    UpdatedAt,
    COUNT(*) OVER() AS Total 
FROM 
    Customers
WHERE UserId = @UserId
ORDER BY
    CreatedAt  DESC
OFFSET (@Page - 1) * @Limit ROWS
FETCH NEXT @Limit ROWS ONLY;
";

        var parameters = new
        {
            UserId = userId, 
            Page = page, 
            Limit = limit, 
        };

        using var connection = _context.CreateConnection();
        
        var totalCount = 0;
            
        var result = await connection.QueryAsync<CustomerEntity, int, CustomerEntity>(
            sql,
            (customer, total) =>
            {
                totalCount = total;
                return customer;
            },
            parameters,
            splitOn: "Total"
        );

        if (!result.Any())
            return new PagedSearchResult<CustomerEntity>(new List<CustomerEntity>(), 0);

        var customers = result.ToList();

        return new PagedSearchResult<CustomerEntity>(
            customers,
            totalCount
        );
    }
    
}