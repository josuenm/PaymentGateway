namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PriceResponse(
    string Id, 
    string Name, 
    long Amount, 
    string Currency, 
    string ProductId, 
    bool IsActive,
    string Frequency, 
    string? Cycle
);