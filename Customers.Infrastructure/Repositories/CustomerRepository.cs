using Customers.Application.Customers.Entities;
using Customers.Domain.Commons;
using Customers.Domain.Customers.Repositories;
using Shared.Infrastructure.Contexts;
using Dapper;

namespace Customers.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DapperContext _context;

    public CustomerRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        const string sql = 
@"
INSERT INTO Customers (Id, Email, Name, TaxId, LiveMode, UserId, CreatedAt) 
VALUES (@Id, @Email, @Name, @TaxId, 0, @UserId, @CreatedAt)
";
        
        try
        {
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, customer);
            }

            return customer;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<Customer?> GetByIdAsync(string userId, string id)
    {
        const string sql = "SELECT * FROM Customers WHERE Id = @Id AND UserId = @UserId;";

        try
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id, UserId = userId });
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<PagedSearchResult<Customer>> GetAllAsync(string userId, int page, int limit)
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
        
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var totalCount = 0;
                
                var result = await connection.QueryAsync<Customer, int, Customer>(
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
                    return new PagedSearchResult<Customer>(new List<Customer>(), 0);

                var customers = result.ToList();

                return new PagedSearchResult<Customer>(
                    customers,
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