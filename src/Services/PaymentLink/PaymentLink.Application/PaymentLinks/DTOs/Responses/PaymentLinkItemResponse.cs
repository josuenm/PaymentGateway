namespace PaymentLink.Application.PaymentLinks.DTOs.Responses;

public record PaymentLinkItemResponse(
    string Id,
    string PriceId, 
    int Quantity
);