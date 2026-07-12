namespace Checkout.Application.CustomerReadModels.Messaging.Commands;

public record CreateCustomerCommand(
    string Id, 
    string Email, 
    string? Name, 
    string? TaxId,
    string UserId,
    bool LiveMode
);