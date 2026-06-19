using Auth.Domain.Roles.Entities;
using Auth.Domain.Roles.Repositories;
using Shared.Infrastructure.Contexts;
using Dapper;

namespace Auth.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly DapperContext _context;

    public RoleRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetRoleByName(string name)
    {
        const string sql = "SELECT * FROM Roles WHERE Name = @Name";
        
        try
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Name = name });
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
}