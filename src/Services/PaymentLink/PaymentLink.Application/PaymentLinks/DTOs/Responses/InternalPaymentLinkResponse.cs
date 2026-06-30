namespace PaymentLink.Application.PaymentLinks.DTOs.Responses;

public record InternalPaymentLinkResponse(
    bool IsActive, 
    IEnumerable<InternalPaymentLinkItemResponse> Items, 
    string UserId
);