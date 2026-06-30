namespace PaymentLink.Application.PaymentLinks.DTOs.Responses;

public record InternalPaymentLinkResponse(
    IEnumerable<InternalPaymentLinkItemResponse> Items,
    bool IsActive,
    bool LiveMode,
    string UserId
);