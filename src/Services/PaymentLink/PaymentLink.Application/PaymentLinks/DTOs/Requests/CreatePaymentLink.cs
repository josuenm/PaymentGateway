using PaymentLink.Domain.PaymentLinks.Enums;

namespace PaymentLink.Application.PaymentLinks.DTOs.Requests;

public record CreatePaymentLink(
    IEnumerable<PaymentLinkMethods> Methods, 
    IEnumerable<CreatePaymentLinkItem> Items
);

public record CreatePaymentLinkItem(
    string PriceId, 
    int Quantity
);