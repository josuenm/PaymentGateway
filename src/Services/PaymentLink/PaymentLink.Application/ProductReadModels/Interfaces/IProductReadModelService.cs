using PaymentLink.Application.ProductReadModels.Messaging.Events;

namespace PaymentLink.Application.ProductReadModels.Interfaces;

public interface IProductReadModelService
{
    public Task CreateFromExternalRequestAsync(ProductCreatedEvent @event);
}