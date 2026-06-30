namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PaymentLinkDetailsResponse(
    IEnumerable<ItemResponse> items, 
    bool IsActive, 
    bool LiveMode
);