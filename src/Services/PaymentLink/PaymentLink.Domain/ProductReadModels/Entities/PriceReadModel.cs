using PaymentLink.Domain.ProductReadModels.Enums;

namespace PaymentLink.Domain.ProductReadModels.Entities;

public class PriceReadModel
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public PriceReadModelFrequency Frequency { get; private set; }
    public PriceReadModelCycle? Cycle { get; private set; }
    public string ProductId { get; private set; }
    public long Amount { get; private set; }
    public string Currency { get; private set; }
    public string UserId { get; private set; }
    public bool LiveMode { get; private set; }

    public PriceReadModel(
        string id, 
        string name,
        PriceReadModelFrequency frequency,
        PriceReadModelCycle? cycle,
        string productId, 
        long amount, 
        string currency, 
        string userId, 
        bool liveMode
    )
    {
        Id = id;
        Name = name;
        Frequency = frequency;
        Cycle = cycle;
        ProductId = productId;
        Amount = amount;
        Currency = currency;
        UserId = userId;
        LiveMode = liveMode;
    }
    
    private PriceReadModel()
    {}
}