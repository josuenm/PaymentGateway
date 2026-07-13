using Catalog.Domain.Prices.Enums;

namespace Catalog.Application.Products.Messaging.Commands;

public record PriceCreatedEvent(
    string Id,
    string Name,
    PriceFrequency Frequency,
    PriceCycle? Cycle,
    string ProductId,
    long Amount,
    string Currency, 
    string UserId,
    bool LiveMode
);