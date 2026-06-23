using System.Text.Json;
using MassTransit;
using PaymentLink.Application.PriceReplicas.DTOs.Requests;
using PaymentLink.Application.PriceReplicas.Interfaces;
using Shared.IntegrationEvents;

namespace PaymentLink.API.Messaging.Consumers;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly IPriceReplicaService _priceReplicaService;

    public ProductCreatedConsumer(IPriceReplicaService priceReplicaService)
    {
        _priceReplicaService = priceReplicaService;
    }
    
    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var message = context.Message;

        var prices = message.Prices.Select(price => new CreatePriceReplicaRequest(
            price.Id, 
            price.UserId
        ));

        await _priceReplicaService.CreateAsync(prices);
    }
}