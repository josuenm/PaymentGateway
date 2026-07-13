namespace Catalog.Application.Products.Messaging.Commands;

public record ProductCreatedEvent(
    string Id,
    IEnumerable<PriceCreatedEvent> Prices,
    string UserId,
    bool LiveMode
);