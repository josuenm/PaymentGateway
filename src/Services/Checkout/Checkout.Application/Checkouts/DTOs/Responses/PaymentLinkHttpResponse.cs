namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PaymentLinkHttpResponse(
    IEnumerable<PaymentLinkItemHttpResponse> Items, 
    bool IsActive,
    bool LiveMode,
    string UserId
);