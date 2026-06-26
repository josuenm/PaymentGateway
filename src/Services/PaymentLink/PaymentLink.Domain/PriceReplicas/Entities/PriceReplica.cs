namespace PaymentLink.Domain.PriceReplicas.Entities;

public class PriceReplica
{
    public string Id { get; private set; }
    public string UserId { get; private set; }

    public PriceReplica(string id, string userId)
    {
        Id = id;
        UserId = userId;
    }
    
    private PriceReplica()
    {}
}