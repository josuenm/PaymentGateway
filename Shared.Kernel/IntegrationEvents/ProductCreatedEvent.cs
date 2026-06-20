namespace Shared.IntegrationEvents;

public record ProductCreatedEvent(
    string Id, 
    string Name, 
    string? Description, 
    bool LiveMode, 
    bool IsActive, 
    string UserId,
    IEnumerable<IntegrationPriceDto> Prices, 
    object? Metadata
);

public record IntegrationPriceDto(
    string Id, 
    string Name, 
    decimal AmountInCents, 
    string Currency,
    string Frequency,
    string? Cycle
);