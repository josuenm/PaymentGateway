using Checkout.Application.PaymentLinkReadModels.Messaging.Events;
using Checkout.Application.PaymentLinkReadModels.Interfaces;
using Checkout.Domain.PaymentLinkReadModels.Entities;
using Checkout.Domain.PaymentLinkReadModels.Repositories;

namespace Checkout.Application.PaymentLinkReadModels.Services;

public class PaymentLinkReadModelService : IPaymentLinkReadModelService
{
    private readonly IPaymentLinkReadModelRepository _paymentLinkReadModelRepository;

    public PaymentLinkReadModelService(IPaymentLinkReadModelRepository paymentLinkReadModelRepository)
    {
        _paymentLinkReadModelRepository = paymentLinkReadModelRepository;
    }

    public async Task<PaymentLinkReadModel?> GetByIdAsync(string id)
    {
        return await _paymentLinkReadModelRepository.GetByIdAsync(id);
    }

    public async Task CreateFromExternalRequestAsync(PaymentLinkCreatedEvent @event)
    {
        var paymentLinkReadModel = new PaymentLinkReadModel(
            @event.Id,
            @event.IsActive,
            @event.UserId,
            @event.LiveMode
        );

        var paymentLinkItems = @event.Items.Select(i =>
            new PaymentLinkItemReadModel(i.Id, i.PaymentLinkId, i.PriceId, i.Quantity, i.LiveMode)
        );
        
        paymentLinkReadModel.SetItems(paymentLinkItems);

        await _paymentLinkReadModelRepository.CreateAsync(paymentLinkReadModel);
    }
}