using Checkout.Domain.ProductReadModels.Enums;

namespace Checkout.Application.ProductReadModels.Messaging.Events;

public record PriceCreatedEvent(
    string Id,
    string Name,
    PriceReadModelFrequency Frequency,
    PriceReadModelCycle? Cycle,
    string ProductId,
    long Amount,
    string Currency, 
    string UserId,
    bool LiveMode
);