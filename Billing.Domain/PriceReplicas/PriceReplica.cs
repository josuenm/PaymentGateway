using Billing.Domain.PriceReplicas.Enums;

namespace Billing.Domain.PriceReplicas;

public class PriceReplica
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public long AmountInCents { get; private set; }
    public string Currency { get; private set; }
    public string ProductReplicaId { get; private set; }
    public bool IsActive { get; private set; }
    public bool LiveMode { get; private set; }
    public PriceReplicaFrequency Frequency { get; private set; }
    public PriceReplicaCycle? Cycle { get; private set; }

    public PriceReplica(
        string id, 
        string name, 
        long amountInCents, 
        string currency, 
        string productReplicaId, 
        bool isActive,
        bool liveMode,
        PriceReplicaFrequency frequency, 
        PriceReplicaCycle? cycle
    )
    {
        Id = id;
        Name = name;
        AmountInCents = amountInCents;
        Currency = currency;
        ProductReplicaId = productReplicaId;
        IsActive = isActive;
        LiveMode = liveMode;
        Frequency = frequency;
        Cycle = cycle;
    }
    
    private PriceReplica()
    {}
}