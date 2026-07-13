namespace Customer.Application.Customers.Messaging.Commands;

public record CustomerCreatedEvent(
    string Id, 
    string Email, 
    string? Name, 
    string? TaxId,
    string UserId,
    bool LiveMode
);