namespace Checkout.Application.Checkouts.DTOs.Responses;

public record CustomerHttpResponse(
    string Id,
    string Email, 
    string Name, 
    string TaxId, 
    DateTime CreatedAt,
    DateTime? UpdatedAt = null
);