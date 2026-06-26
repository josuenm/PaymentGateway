using Billing.Domain.PriceReplicas;

namespace Billing.Domain.ProductReplicas;

public class ProductReplica
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool LiveMode { get; private set; }
    public bool IsActive { get; private set; }
    public string UserId { get; private set; }
    public object? Metadata { get; private set; }
    
    public IEnumerable<PriceReplica>? Prices { get; private set; }

    public ProductReplica(
        string id, 
        string name, 
        string? description, 
        bool liveMode, 
        bool isActive, 
        string userId, 
        object? metadata
    )
    {
        Id = id;
        Name = name;
        Description = description;
        LiveMode = liveMode;
        IsActive = isActive;
        UserId = userId;
        Metadata = metadata;
    }

    public void SetPrices(IEnumerable<PriceReplica> prices)
    {
        Prices = prices;
    }
    
    private ProductReplica()
    {}
}