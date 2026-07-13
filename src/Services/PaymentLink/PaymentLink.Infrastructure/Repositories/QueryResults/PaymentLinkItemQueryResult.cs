namespace PaymentLink.Infrastructure.Repositories.QueryResults;

internal class PaymentLinkItemQueryResult
{
    public string PaymentLinkItemId { get; set; } = null!;
    public string PriceId { get; set; } = null!;
    public string PaymentLinkItemUserId { get; set; } = null!;
    public string PaymentLinkId { get; set; } = null!;
    public bool PaymentLinkItemLiveMode { get; set; }
    public int Quantity { get; set; }
    public DateTime PaymentLinkItemCreatedAt { get; set; }
    public DateTime PaymentLinkItemUpdatedAt { get; set; }
}