using MassTransit;
using Shared.IntegrationEvents;

namespace Billing.Infrastructure.Messaging.Consumers;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        
    }
}