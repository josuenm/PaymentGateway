namespace Checkout.Application.Checkouts.DTOs.Requests;

public record CreateCustomerHttpRequest(
    string Email,
    string Name,
    string TaxId
);