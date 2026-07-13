using Checkout.Application.ProductReadModels.Messaging.Events;

namespace Checkout.Application.ProductReadModels.Interfaces;

public interface IProductReadModelService
{
    public Task CreateFromExternalRequestAsync(ProductCreatedEvent @event);
}