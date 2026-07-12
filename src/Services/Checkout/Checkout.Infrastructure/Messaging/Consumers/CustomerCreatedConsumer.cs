using Checkout.Application.CustomerReadModels.Messaging.Events;
using Checkout.Application.CustomerReadModels.Interfaces;
using MassTransit;

namespace Checkout.Infrastructure.Messaging.Consumers;

public class CustomerCreatedConsumer : IConsumer<CustomerCreatedEvent>
{
    
    private readonly ICustomerReadModelService _customerReadModelService;

    public CustomerCreatedConsumer(ICustomerReadModelService customerReadModelService)
    {
        _customerReadModelService = customerReadModelService;
    }
    
    public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
    {
        await _customerReadModelService.CreateFromExternalRequestAsync(context.Message);        
    }
}