namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PaymentLinkResponse(
    IEnumerable<PaymentLinkItemResponse> Items, 
    bool IsActive
);