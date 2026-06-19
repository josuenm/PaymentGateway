using Catalog.Domain.Enums;
using Catalog.Domain.Products.Entities;
using Shared.Kernel.Utils;

namespace Catalog.Domain.Prices.Entities;

public class Price
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public string ProductId { get; private set; }
    public string UserId { get; private set; }
    public PriceFrequency Frequency { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    public Product? Product { get; private set; }

    public Price(string name, decimal amount, string currency, PriceFrequency frequency, string productId, string userId)
    {
        Id = IdGenerator.Generate("pri");
        Name = name;
        Amount = amount;
        Currency = currency;
        Frequency = frequency;
        UserId = userId;
        ProductId = productId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public Price(
        string id, 
        string name,
        decimal amount, 
        string currency, 
        string productId,
        PriceFrequency frequency, 
        string userId,
        DateTime createdAt, 
        DateTime? updatedAt = null
    )
    {
        Id = id;
        Name = name;
        Amount = amount;
        Currency = currency;
        ProductId = productId;
        Frequency = frequency;
        UserId = userId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public void SetProduct(Product product)
    {
        Product = product;
    }
    
    private Price() {}
}