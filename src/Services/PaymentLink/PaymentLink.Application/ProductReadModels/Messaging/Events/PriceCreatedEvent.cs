using PaymentLink.Domain.ProductReadModels.Enums;

namespace PaymentLink.Application.ProductReadModels.Messaging.Events;

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