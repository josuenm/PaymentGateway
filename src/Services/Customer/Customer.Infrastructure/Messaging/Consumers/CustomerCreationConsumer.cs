using Customer.Application.Customers.Messaging.Events;
using Customer.Application.Customers.Interfaces;
using MassTransit;

namespace Customer.Infrastructure.Messaging.Consumers;

public class CustomerCreationConsumer : IConsumer<CustomerCreatedEvent>
{
    private readonly ICustomerService _customerService;
    
    public CustomerCreationConsumer(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
    {
        await _customerService.CreateFromExternalRequestAsync(context.Message);
    }
}