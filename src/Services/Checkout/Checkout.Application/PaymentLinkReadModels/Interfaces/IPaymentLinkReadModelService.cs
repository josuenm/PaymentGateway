using Checkout.Application.PaymentLinkReadModels.Events.Events;
using Checkout.Domain.PaymentLinkReadModels.Entities;

namespace Checkout.Application.PaymentLinkReadModels.Interfaces;

public interface IPaymentLinkReadModelService
{
    public Task CreateFromExternalRequestAsync(PaymentLinkCreatedEvent @event);
    public Task<PaymentLinkReadModel?> GetByIdAsync(string id);
}