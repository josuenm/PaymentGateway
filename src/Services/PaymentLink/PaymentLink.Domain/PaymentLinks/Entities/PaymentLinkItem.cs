using Shared.Kernel.Utils;

namespace PaymentLink.Domain.PaymentLinks.Entities;

public class PaymentLinkItem
{
    public string Id { get; private set; }
    public string UserId { get; private set; }
    public string PaymentLinkId { get; private set; }
    public string PriceId { get; private set; }
    public int Quantity { get; private set; }
    public bool LiveMode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public PaymentLinkItem(string priceId, string userId, string paymentLinkId, int quantity, bool liveMode)
    {
        Id = IdGenerator.Generate("pli");
        UserId = userId;
        PaymentLinkId = paymentLinkId;
        PriceId = priceId;
        Quantity = quantity;
        LiveMode = liveMode;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }
    
    public PaymentLinkItem(
        string id, 
        string priceId, 
        string userId, 
        string paymentLinkId, 
        int quantity, 
        bool liveMode,
        DateTime createdAt,
        DateTime updatedAt
    )
    {
        Id = id;
        UserId = userId;
        PaymentLinkId = paymentLinkId;
        PriceId = priceId;
        Quantity = quantity;
        LiveMode = liveMode;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    
    private PaymentLinkItem()
    {}
}