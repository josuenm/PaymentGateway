using Catalog.Domain.Prices.Entities;
using Shared.Kernel.Utils;

namespace Catalog.Domain.Products.Entities;

public class Product
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string UserId { get; private set; }
    public bool IsActive { get; private set; }
    public bool LiveMode { get; private set; }
    public string? Description { get; private set; }
    public object? Metadata { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    public IEnumerable<Price>? Prices { get; private set; }

    public Product(
        string name, 
        string userId, 
        bool liveMode, 
        bool isActive, 
        string? description = null, 
        object? metadata = null
    )
    {
        Id = IdGenerator.Generate("prod");
        Name = name;
        UserId = userId;
        Description = description;
        LiveMode = liveMode;
        IsActive = isActive;
        Metadata = metadata;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public Product(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        UserId = product.UserId;
        Description = product.Description ?? null;
        LiveMode = product.LiveMode;
        IsActive = product.IsActive;
        Metadata = product.Metadata ?? null;
        CreatedAt = product.CreatedAt;
        UpdatedAt = product.UpdatedAt;
    }

    public void Update(string? name, string? description = null, object? metadata = null)
    {
        Name = name ?? "";
        Description = description;
        Metadata = metadata;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Disable()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Enable()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPrices(IEnumerable<Price> prices)
    {
        Prices = prices;
    }

    private Product() {}
}