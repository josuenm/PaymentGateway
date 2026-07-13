namespace Checkout.Domain.PaymentLinkReadModels.Entities;

public class PaymentLinkReadModel
{
    public string Id { get; private set; }
    public bool IsActive { get; private set; }
    public string UserId { get; private set; }
    public bool LiveMode { get; private set; }
    public IEnumerable<PaymentLinkItemReadModel> Items { get; private set; }

    public PaymentLinkReadModel(string id, bool isActive, string userId, bool liveMode)
    {
        Id = id;
        IsActive = isActive;
        UserId = userId;
        LiveMode = liveMode;
    }

    public void SetItems(IEnumerable<PaymentLinkItemReadModel> items)
    {
        Items = items;
    }
    
    private PaymentLinkReadModel()
    {}
}