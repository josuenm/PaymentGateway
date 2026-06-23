using PaymentLink.Application.PaymentLinks.DTOs.Requests;
using PaymentLink.Application.PaymentLinks.DTOs.Responses;
using PaymentLink.Application.PaymentLinks.Interfaces;
using PaymentLink.Domain.Entities;
using PaymentLink.Domain.PaymentLinks.Repositories;
using PaymentLink.Domain.PriceReplicas.Repositories;
using Shared.Kernel.Results;

namespace PaymentLink.Application.PaymentLinks.Services;

public class PaymentLinkService : IPaymentLinkService
{
    private readonly IPaymentLinkRepository _paymentLinkRepository;
    private readonly IPriceReplicaRepository _priceReplicaRepository;

    public PaymentLinkService(
        IPaymentLinkRepository paymentLinkRepository, 
        IPriceReplicaRepository priceReplicaRepository
    )
    {
        _paymentLinkRepository = paymentLinkRepository;
        _priceReplicaRepository = priceReplicaRepository;
    }
    
    public async Task<Result<PaymentLinkResponse>> CreateAsync(CreatePaymentLink request, string userId)
    {
        var pricesIdList = request.Items.Select(item => item.PriceId).Distinct().ToList();
        var pricesFound = (await _priceReplicaRepository.GetManyById(pricesIdList)).ToList();

        if (pricesIdList.Count != pricesFound.Count)
        {
            return Result<PaymentLinkResponse>.Failure("Algum preço é inválido", ErrorType.Validation);
        }

        foreach (var price in request.Items)
        {
            var item = pricesFound.FirstOrDefault(item => item.Id == price.PriceId);

            if (item == null)
            {
                return Result<PaymentLinkResponse>.Failure("Preço não encontrado", ErrorType.NotFound);
            }

            if (item.UserId != userId)
            {
                return Result<PaymentLinkResponse>.Failure("Preço inválido", ErrorType.Validation);
            }
        }

        var paymentLink = new PaymentLinkEntity(
            userId, 
            request.Methods, 
            true, 
            true
        );

        if (request.Items.Any())
        {
            var items = request.Items.Select(item => new PaymentLinkItem(
                item.PriceId, 
                userId,
                paymentLink.Id,
                item.Quantity,
                true
            )).ToList();
            paymentLink.SetItems(items);
        }

        await _paymentLinkRepository.CreateAsync(paymentLink);

        return Result<PaymentLinkResponse>.Created(new PaymentLinkResponse(
            paymentLink.Id, 
            paymentLink.LiveMode, 
            paymentLink.IsActive, 
            paymentLink.Methods, 
            paymentLink.Items.Any() 
                ? paymentLink.Items.Select(item => new PaymentLinkItemResponse(
                    item.Id,
                    item.PriceId, 
                    item.Quantity
                  ))
                : new List<PaymentLinkItemResponse>(), 
            paymentLink.CreatedAt, 
            paymentLink.UpdatedAt
        ));
    }

    public async Task<Result<InternalPaymentLinkResponse>> GetInternalByIdAsync(string id)
    {
        var paymentLink = await _paymentLinkRepository.GetByIdAsync(id, true);

        if (paymentLink == null)
        {
            return Result<InternalPaymentLinkResponse>
                .Failure("Link de pagamento não encontrado", ErrorType.NotFound);
        }

        return Result<InternalPaymentLinkResponse>.Ok(new InternalPaymentLinkResponse(
            paymentLink.IsActive, 
            paymentLink.Items.Any()
                ? paymentLink.Items.Select(item => new InternalPaymentLinkItemResponse(item.PriceId, item.Quantity))
                : new List<InternalPaymentLinkItemResponse>()
        ));
    }
}