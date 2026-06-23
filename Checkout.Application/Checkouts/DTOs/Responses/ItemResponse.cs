namespace Checkout.Application.Checkouts.DTOs.Responses;

public record ItemResponse(
    string ProductId,
    string PriceId,
    string Name,
    int Quantity,
    string Currency,
    long Amount,
    string Frequency, 
    string Cycle 
);