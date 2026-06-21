using Billing.Application.PriceReplicas.DTOs.Requests;
using Billing.Application.ProductReplica.Interfaces;
using Billing.Application.ProductReplicas.DTOs.Requests;
using Billing.Domain.PriceReplicas.Enums;
using MassTransit;
using Shared.IntegrationEvents;

namespace Billing.API.Messaging.Consumers;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly IProductReplicaService _productReplicaService;

    public ProductCreatedConsumer(IProductReplicaService productReplicaService)
    {
        _productReplicaService = productReplicaService;
    }
    
    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var message = context.Message;

        var pricesToReplica = new List<CreatePriceReplicaRequest>();

        foreach (var price in message.Prices)
        {
            if (Enum.TryParse<PriceReplicaFrequency>(price.Frequency, out var frequency))
            {
                PriceReplicaCycle? cycle = null;
                if (!string.IsNullOrEmpty(price.Cycle) && Enum.TryParse<PriceReplicaCycle>(price.Cycle, out var parsedCycle))
                {
                    cycle = parsedCycle;                    
                }
                
                pricesToReplica.Add(new CreatePriceReplicaRequest(
                    price.Id, 
                    price.Name, 
                    price.AmountInCents, 
                    price.Currency, 
                    message.Id, 
                    price.IsActive, 
                    price.LiveMode, 
                    frequency, 
                    cycle
                ));
            }

            var productReplica = new CreateProductReplicaRequest(
                message.Id, 
                message.Name, 
                message.Description, 
                message.LiveMode, 
                message.IsActive,
                message.UserId,
                pricesToReplica, 
                message.Metadata
            );
            
            await _productReplicaService.CreateAsync(productReplica);
        }
    }
}