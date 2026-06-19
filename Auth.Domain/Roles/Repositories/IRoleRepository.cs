using Auth.Domain.Roles.Entities;

namespace Auth.Domain.Roles.Repositories;

public interface IRoleRepository
{
    public Task<Role?> GetRoleByName(string name);
}