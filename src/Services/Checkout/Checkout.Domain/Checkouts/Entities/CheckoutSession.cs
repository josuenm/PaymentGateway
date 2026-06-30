using Checkout.Domain.Checkouts.Enums;
using Shared.Kernel.Utils;

namespace Checkout.Domain.Checkouts.Entities;

public class CheckoutSession
{
    public string Id { get; private set; }
    public string CustomerId { get; private set; }
    public string PaymentId { get; private set; }
    public string PaymentLinkId { get; private set; }
    public long Amount { get; private set; }
    public string Currency { get; private set; }
    public string UserId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public CheckoutSession(
        string customerId, 
        string paymentId, 
        string paymentLinkId, 
        long amount, 
        string currency, 
        string userId,
        PaymentMethod paymentMethod 
    )
    {
        Id = IdGenerator.Generate("sess");
        CustomerId = customerId;
        PaymentId = paymentId;
        PaymentLinkId = paymentLinkId;
        Amount = amount;
        Currency = currency;
        UserId = userId;
        PaymentMethod = paymentMethod;
        CreatedAt = DateTime.UtcNow;
    }
    
    private CheckoutSession() { }
}