namespace PaymentLink.Application.PaymentLinks.DTOs.Responses;

public record InternalPaymentLinkItemResponse(
    string PriceId, 
    int Quantity
);