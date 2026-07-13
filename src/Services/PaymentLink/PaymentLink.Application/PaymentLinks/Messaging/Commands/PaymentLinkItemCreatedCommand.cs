namespace PaymentLink.Application.PaymentLinks.Messaging.Commands;

public record PaymentLinkItemCreatedCommand(
    string Id,
    string PaymentLinkId,
    string PriceId,
    bool LiveMode
);