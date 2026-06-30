namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PaymentLinkItemHttpResponse(
    string PriceId, 
    int Quantity
);