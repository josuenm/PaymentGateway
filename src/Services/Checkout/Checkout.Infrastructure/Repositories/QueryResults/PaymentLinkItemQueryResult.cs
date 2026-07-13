namespace Checkout.Infrastructure.Repositories.QueryResults;

internal class PaymentLinkItemQueryResult
{
    public string PaymentLinkItemId { get; set; } = null!;
    public string PriceId { get; set; } = null!;
    public int Quantity { get; set; }
    public string PaymentLinkId { get; set; } = null!;
    public bool PaymentLinkItemLiveMode { get; set; }
}