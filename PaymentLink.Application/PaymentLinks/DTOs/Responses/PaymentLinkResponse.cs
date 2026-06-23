using PaymentLink.Domain.PaymentLinks.Enums;

namespace PaymentLink.Application.PaymentLinks.DTOs.Responses;

public record PaymentLinkResponse(
    string Id, 
    bool LiveMode, 
    bool IsActive, 
    IEnumerable<PaymentLinkMethods> Methods,
    IEnumerable<PaymentLinkItemResponse> Items, 
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record PaymentLinkItemResponse(
    string Id,
    string PriceId, 
    int Quantity
);