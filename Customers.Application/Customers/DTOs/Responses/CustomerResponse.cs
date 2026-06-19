namespace Customers.Application.Customers.DTOs.Responses;

public record CustomerResponse(
    string Id,
    string Email, 
    string? Name, 
    string? TaxId, 
    DateTime CreatedAt,
    DateTime? UpdatedAt = null
);