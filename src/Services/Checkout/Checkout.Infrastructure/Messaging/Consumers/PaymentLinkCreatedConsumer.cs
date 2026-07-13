using Checkout.Application.PaymentLinkReadModels.Messaging.Events;
using Checkout.Application.PaymentLinkReadModels.Interfaces;
using MassTransit;

namespace Checkout.Infrastructure.Messaging.Consumers;

public class PaymentLinkCreatedConsumer : IConsumer<PaymentLinkCreatedEvent>
{
    private readonly IPaymentLinkReadModelService _paymentLinkReadModelService;

    public PaymentLinkCreatedConsumer(IPaymentLinkReadModelService paymentLinkReadModelService)
    {
        _paymentLinkReadModelService = paymentLinkReadModelService;
    }

    public async Task Consume(ConsumeContext<PaymentLinkCreatedEvent> context)
    {
        await _paymentLinkReadModelService.CreateFromExternalRequestAsync(context.Message);
    }
}