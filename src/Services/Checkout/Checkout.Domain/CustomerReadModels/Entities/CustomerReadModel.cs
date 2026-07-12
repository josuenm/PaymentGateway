using Shared.Kernel.Utils;

namespace Checkout.Domain.CustomerReadModels.Entities;

public class CustomerReadModel
{
    public string Id { get; private set; }
    public string Email { get; private set; }
    public string? Name { get; private set; }
    public string? TaxId { get; private set; }
    public string UserId { get; private set; }
    public bool LiveMode { get; private set; }

    public CustomerReadModel(string id, string email, string? name, string? taxId, string userId, bool liveMode)
    {
        Id = id;
        Email = email;
        Name = name;
        TaxId = taxId;
        UserId = userId;
        LiveMode = liveMode;
    }
    
    public CustomerReadModel(string email, string? name, string? taxId, string userId, bool liveMode)
    {
        Id = IdGenerator.Generate("cust");;
        Email = email;
        Name = name;
        TaxId = taxId;
        UserId = userId;
        LiveMode = liveMode;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetTaxId(string taxId)
    {
        TaxId = taxId;
    }
    
    private CustomerReadModel()
    {}
}