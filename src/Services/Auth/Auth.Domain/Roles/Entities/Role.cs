using Shared.Kernel.Utils;

namespace Auth.Domain.Roles.Entities;

public class Role
{
    public string Id { get; private set; }
    public string Name { get; private set; }

    public Role(string id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public Role(string name)
    {
        Id = IdGenerator.Generate("role");
        Name = name;
    }

    public void Update(string name)
    {
        Name = name;        
    }
    
    private Role() {}
}