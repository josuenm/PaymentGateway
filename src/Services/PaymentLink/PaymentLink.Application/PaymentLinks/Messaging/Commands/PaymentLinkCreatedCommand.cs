namespace PaymentLink.Application.PaymentLinks.Messaging.Commands;

public record PaymentLinkCreatedCommand(
    string Id,
    bool IsActive,
    string UserId,
    IEnumerable<PaymentLinkItemCreatedCommand> Items, 
    bool LiveMode
);