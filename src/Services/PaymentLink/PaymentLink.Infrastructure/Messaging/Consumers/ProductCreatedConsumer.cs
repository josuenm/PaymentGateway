using MassTransit;
using PaymentLink.Application.ProductReadModels.Interfaces;
using PaymentLink.Application.ProductReadModels.Messaging.Events;

namespace PaymentLink.Infrastructure.Messaging.Consumers;

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