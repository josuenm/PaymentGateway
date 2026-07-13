namespace Checkout.Application.CustomerReadModels.Messaging.Commands;

public record UpdateCustomerCommand(
    string Id, 
    string Email, 
    string? Name, 
    string? TaxId,
    string UserId,
    bool LiveMode
);