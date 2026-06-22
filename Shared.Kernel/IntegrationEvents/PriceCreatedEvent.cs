namespace Shared.IntegrationEvents;

public record PriceCreatedEvent(
    string Id,
    string Name,
    string UserId,
    long AmountInCents,
    string Currency,
    bool IsActive,
    bool LiveMode,
    string Frequency,
    string? Cycle 
);
