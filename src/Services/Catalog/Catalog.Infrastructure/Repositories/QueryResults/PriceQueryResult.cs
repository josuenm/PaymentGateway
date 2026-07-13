using Catalog.Domain.Prices.Enums;

namespace Catalog.Infrastructure.Repositories.QueryResults;

internal class PriceQueryResult
{
    public string PriceId { get; set; }
    public string PriceName { get; set; }
    public string PriceUserId { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public PriceFrequency Frequency { get; set; }
    public PriceCycle Cycle { get; set; }
    public bool PriceIsActive { get; set; }
    public bool PriceLiveMode { get; set; }
    public string ProductId { get; set; }
    public DateTime PriceCreatedAt { get; set; }
    public DateTime PriceUpdatedAt { get; set; }
}