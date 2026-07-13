namespace Checkout.Domain.PaymentLinkReadModels.Entities;

public class PaymentLinkItemReadModel
{
    public string Id { get; private set; }
    public string PaymentLinkId { get; private set; }
    public string PriceId { get; private set; }
    public int Quantity { get; private set; }
    public bool LiveMode { get; private set; }
    
    public PaymentLinkItemReadModel(string id, string paymentLinkId, string priceId, int quantity, bool liveMode)
    {
        Id = id;
        PaymentLinkId = paymentLinkId;
        PriceId = priceId;
        Quantity = quantity;
        LiveMode = liveMode;
    }
    
    private PaymentLinkItemReadModel()
    {}
}