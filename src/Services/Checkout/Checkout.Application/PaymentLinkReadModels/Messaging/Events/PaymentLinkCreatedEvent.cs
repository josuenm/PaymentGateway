namespace Checkout.Application.PaymentLinkReadModels.Messaging.Events;

public record PaymentLinkCreatedEvent(
    string Id,
    bool IsActive,
    string UserId,
    IEnumerable<PaymentLinkItemCreatedEvent> Items, 
    bool LiveMode
);