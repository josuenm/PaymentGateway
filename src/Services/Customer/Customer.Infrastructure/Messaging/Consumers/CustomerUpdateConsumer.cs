using MassTransit;

namespace Customer.Infrastructure.Messaging.Consumers;

public class CustomerUpdateConsumer : IConsumer<CustomerUpdateConsumer>
{
    public async Task Consume(ConsumeContext<CustomerUpdateConsumer> context)
    {
        
    }
}