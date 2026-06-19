namespace Customers.Application.Customers.DTOs.Requests;

public record CreateCustomerRequest(
    string Email,
    string? Name,
    string? TaxId
);