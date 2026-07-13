using Checkout.Domain.CustomerReadModels.Entities;
using Checkout.Domain.CustomerReadModels.Repositories;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace Checkout.Infrastructure.Repositories;

public class CustomerReadModelRepository : ICustomerReadModelRepository
{
    private readonly DapperContext _context;
    
    public CustomerReadModelRepository(DapperContext context)
    {
        _context = context;
    }
    
    public async Task<CustomerReadModel> CreateAsync(CustomerReadModel readModel)
    {
        const string sql = 
@"
INSERT INTO CustomerReadModels (Id, Email, Name, TaxId, UserId, LiveMode)
VALUES (@Id, @Email, @Name, @TaxId, @UserId, @LiveMode);
";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(sql, readModel);

        return readModel;
    }

    public async Task<CustomerReadModel?> GetByEmailAndUserIdAsync(string userId, string email)
    {
        const string sql = "SELECT * FROM CustomerReadModels WHERE Email = @Email AND UserId = @UserId;";
        var parameters = new { Email = email, UserId = userId };

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CustomerReadModel>(sql, parameters);
    }

    public async Task<CustomerReadModel> UpdateAsync(CustomerReadModel readModel)
    {
        const string sql = 
@"
UPDATE CustomerReadModels
SET Email = @Email, Name = @Name, TaxId = @TaxId
WHERE Id = @Id;
";
        
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(sql, readModel);

        return readModel;
    }
}