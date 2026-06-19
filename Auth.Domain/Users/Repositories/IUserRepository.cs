using Auth.Domain.Users.Entities;

namespace Auth.Domain.Users.Repositories;

public interface IUserRepository
{
    public Task<User> CreateAsync(User user);
    public Task<User?> GetUserByEmailAsync(string email);
}