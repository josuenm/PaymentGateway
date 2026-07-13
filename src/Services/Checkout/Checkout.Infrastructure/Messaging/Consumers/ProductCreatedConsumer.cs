using Checkout.Application.ProductReadModels.Interfaces;
using Checkout.Application.ProductReadModels.Messaging.Events;
using MassTransit;

namespace Checkout.Infrastructure.Messaging.Consumers;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly IProductReadModelService _productReadModelService;

    public ProductCreatedConsumer(IProductReadModelService productReadModelService)
    {
        _productReadModelService = productReadModelService;
    }
    
    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        await _productReadModelService.CreateFromExternalRequestAsync(context.Message);
    }
}