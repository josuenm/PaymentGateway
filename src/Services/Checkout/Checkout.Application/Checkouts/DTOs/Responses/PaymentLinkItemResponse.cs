namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PaymentLinkItemResponse(
    string PriceId, 
    int Quantity
);