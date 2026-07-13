using Checkout.Application.ProductReadModels.Interfaces;
using Checkout.Application.ProductReadModels.Messaging.Events;
using Checkout.Domain.ProductReadModels.Entities;
using Checkout.Domain.ProductReadModels.Repositories;

namespace Checkout.Application.ProductReadModels.Services;

public class ProductReadModelService : IProductReadModelService
{
    private readonly IProductReadModelRepository _productReadModelRepository;

    public ProductReadModelService(IProductReadModelRepository productReadModelRepository)
    {
        _productReadModelRepository = productReadModelRepository;
    }

    public async Task CreateFromExternalRequestAsync(ProductCreatedEvent @event)
    {
        var product = new ProductReadModel(@event.Id, @event.UserId, @event.LiveMode);

        if (@event.Prices.Any())
        {
            var prices = @event.Prices.Select(price => new PriceReadModel(
                price.Id, 
                price.Name,
                price.Frequency, 
                price.Cycle, 
                price.ProductId,
                price.Amount,
                price.Currency,
                price.UserId,
                price.LiveMode
            ));
            product.SetPrices(prices);
        }

        await _productReadModelRepository.CreateAsync(product);
    }
}