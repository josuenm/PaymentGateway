namespace Customer.Application.Customers.Messaging.Events;

public record CustomerUpdatedEvent(
    string Id,
    string? Name,
    string Email,
    string? TaxId,
    string UserId,
    bool LiveMode
);