using Shared.Kernel.Utils;

namespace Auth.Domain.Users.Entities;

public class User
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public List<string> Roles { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public User(string id, string name, string email, string password, DateTime  createdAt, DateTime? updatedAt)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    
    public User(string name, string email, string password)
    {
        Id = IdGenerator.Generate("usr");
        Name = name;
        Email = email;
        Password = password;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public void Update(string? name, string? email, string? password)
    {
        Name = name ?? Name;
        Email = email ?? Email;
        Password = password ?? Password;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRoles(IEnumerable<string> roles)
    {
        Roles = roles.ToList();
    }
    
    private User() {}
}