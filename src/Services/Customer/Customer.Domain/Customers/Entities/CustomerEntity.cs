using Shared.Kernel.Utils;

namespace Customer.Domain.Customers.Entities;

public class CustomerEntity
{
    public string Id { get; private set; }
    public string Email { get; private set; }
    public string? Name { get; private set; }
    public string? TaxId { get; private set; }
    public bool LiveMode { get; private set; }
    public string UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public CustomerEntity(CustomerEntity customer)
    {
        Id = customer.Id;
        Email = customer.Email;
        Name = customer.Name;
        TaxId = customer.TaxId;
        LiveMode = customer.LiveMode;
        UserId = customer.UserId;
        CreatedAt = customer.CreatedAt;
        UpdatedAt = customer.UpdatedAt;
    }

    public CustomerEntity(string email, string? name, string? taxId, bool liveMode, string userId)
    {
        Id = IdGenerator.Generate("cust");
        Name = name;
        Email = email;
        TaxId = taxId;
        LiveMode = liveMode;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public static CustomerEntity CreateWithId(
        string id, 
        string email, 
        string? name, 
        string? taxId, 
        string userId,
        bool liveMode 
    )
    {
        var customer = new CustomerEntity(email, name, taxId, liveMode, userId);
        customer.SetId(id);
        return customer;
    }

    public void SetId(string id)
    {
        Id = id;
    }

    public void Update(string? name, string? taxId)
    {
        Name = name;
        TaxId = taxId;
        UpdatedAt = DateTime.UtcNow;
    }
    
    private CustomerEntity() {}
}