namespace Checkout.Application.CustomerReadModels.Messaging.Events;

public record CustomerCreatedEvent(
    string Id, 
    string Email, 
    string? Name, 
    string? TaxId,
    string UserId,
    bool LiveMode
);