namespace PaymentLink.Application.ProductReadModels.Messaging.Events;

public record ProductCreatedEvent(
    string Id,
    IEnumerable<PriceCreatedEvent> Prices,
    string UserId,
    bool LiveMode
);