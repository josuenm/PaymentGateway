namespace Checkout.Application.PaymentLinkReadModels.Events.Events;

public record PaymentLinkItemCreatedEvent(
    string Id,
    string PaymentLinkId,
    string PriceId,
    int Quantity,
    bool LiveMode
);