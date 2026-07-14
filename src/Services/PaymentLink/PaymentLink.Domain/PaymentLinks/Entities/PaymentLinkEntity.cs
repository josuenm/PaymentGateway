using PaymentLink.Domain.PaymentLinks.Enums;
using Shared.Kernel.Utils;

namespace PaymentLink.Domain.PaymentLinks.Entities;

public class PaymentLinkEntity
{
    public string Id { get; private set; }
    public string UserId { get; private set; }
    public IReadOnlyCollection<PaymentLinkMethods> Methods { get; private set; }
    public IEnumerable<PaymentLinkItem> Items { get; private set; }
    public bool LiveMode { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public PaymentLinkEntity(string userId, IReadOnlyCollection<PaymentLinkMethods> methods, bool liveMode, bool isActive)
    {
        Id = IdGenerator.Generate("plink");
        UserId = userId;
        Methods = methods;
        LiveMode = liveMode;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public void SetItems(IEnumerable<PaymentLinkItem> items)
    {
        Items = items;
    }
    
    private PaymentLinkEntity()
    {}
}