namespace Customer.Application.Customers.Messaging.Commands;

public record CustomerCreatedCommand(
    string Id, 
    string? Name, 
    string Email, 
    string? TaxId
);