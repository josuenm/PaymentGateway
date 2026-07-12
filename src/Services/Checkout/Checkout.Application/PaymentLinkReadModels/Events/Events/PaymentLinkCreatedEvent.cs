namespace Checkout.Application.PaymentLinkReadModels.Events.Events;

public record PaymentLinkCreatedEvent(
    string Id,
    bool IsActive,
    string UserId,
    IEnumerable<PaymentLinkItemCreatedEvent> Items, 
    bool LiveMode
);