namespace Customer.Application.Customers.Messaging.Events;

public record CustomerCreatedEvent(
    string Id,
    string? Name,
    string Email,
    string? TaxId,
    string UserId,
    bool LiveMode
);