using Auth.Domain.Users.Entities;
using Auth.Domain.Users.Repositories;
using Shared.Infrastructure.Contexts;
using Dapper;

namespace Auth.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    public readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<User> CreateAsync(User user)
    {
        const string sql =
@"
INSERT INTO Users (Id, Name, Email, Password, CreatedAt)
VALUES (@Id, @Name, @Email, @Password, @CreatedAt)
";
        using var connection = _context.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            await connection.ExecuteAsync(sql, user, transaction);

            if (user.Roles.Any())
            {
                const string getRolesBatchSql = "SELECT Id FROM Roles WHERE Name IN @RoleNames";
            
                var roleIds = await connection.QueryAsync<string>(
                    getRolesBatchSql, 
                    new { RoleNames = user.Roles }, 
                    transaction
                );

                if (roleIds.Any())
                {
                    const string insertRolesSql = "INSERT INTO UserRoles (RoleId, UserId) VALUES (@RoleId, @UserId)";
                    var userRolesParameters = roleIds.Select(roleId => new
                    {
                        UserId = user.Id, 
                        RoleId = roleId
                    });
                    
                    await connection.ExecuteAsync(insertRolesSql, userRolesParameters, transaction);
                }
            }
            transaction.Commit();
            
            return user;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        string sql = 
@"
SELECT u.Id, u.Name, u.Email, u.Password, u.CreatedAt, u.UpdatedAt, r.Name as RoleName 
FROM Users u
INNER JOIN UserRoles ur ON u.Id = ur.UserId
INNER JOIN Roles r ON ur.RoleId = r.Id
WHERE u.Email = @Email
";

        using var connection = _context.CreateConnection();
        var userDictionary = new Dictionary<string, User>();

        await connection.QueryAsync<User, string, User>(
            sql,
            (user, roleName) =>
            {
                if (!userDictionary.TryGetValue(user.Id, out var existingUser))
                {
                    existingUser = user;
                    existingUser.SetRoles(new List<string>());
                    userDictionary.Add(user.Id, user);
                }

                if (!string.IsNullOrEmpty(roleName))
                {
                    var currentRoles = existingUser.Roles;
                    if (!currentRoles.Contains(roleName))
                    {
                        currentRoles.Add(roleName);
                        existingUser.SetRoles(currentRoles);
                    }
                }

                return existingUser;
            },
            new { Email = email }, 
            splitOn: "RoleName"
        );

        return userDictionary.Values.FirstOrDefault();
    }
}