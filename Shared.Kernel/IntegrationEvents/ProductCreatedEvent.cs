namespace Shared.IntegrationEvents;

public record ProductCreatedEvent(
    string Id, 
    string Name, 
    string? Description, 
    bool LiveMode, 
    bool IsActive, 
    string UserId,
    IEnumerable<PriceCreatedEvent> Prices, 
    object? Metadata = null
);