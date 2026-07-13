using MassTransit;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;
using PaymentLink.Application.PaymentLinks.DTOs.Responses;
using PaymentLink.Application.PaymentLinks.Interfaces;
using PaymentLink.Application.PaymentLinks.Messaging.Commands;
using PaymentLink.Application.ProductReadModels.Interfaces;
using PaymentLink.Domain.Entities;
using PaymentLink.Domain.PaymentLinks.Repositories;
using Shared.DTOs.Responses;
using Shared.Kernel.Results;

namespace PaymentLink.Application.PaymentLinks.Services;

public class PaymentLinkService : IPaymentLinkService
{
    private readonly IPriceReadModelService _priceReadModelService;
    private readonly IPaymentLinkRepository _paymentLinkRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentLinkService(
        IPriceReadModelService priceReadModelService,
        IPaymentLinkRepository paymentLinkRepository, 
        IPublishEndpoint publishEndpoint
    )
    {
        _priceReadModelService = priceReadModelService;
        _paymentLinkRepository = paymentLinkRepository;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task<Result<PaymentLinkResponse>> CreateAsync(CreatePaymentLink request, string userId)
    {
        var priceIdList = request.Items.Select(item => item.PriceId).Distinct().ToList();
        
        var prices = await _priceReadModelService.GetManyByIdAsync(priceIdList);
        
        if (priceIdList.Count != prices.Count())
        {
            return Result<PaymentLinkResponse>.BadRequest("Algum preço é inválido");
        }

        var priceList = prices.ToList();

        foreach (var price in request.Items)
        {
            var item = priceList.FirstOrDefault(item => item.Id == price.PriceId);

            if (item == null)
            {
                return Result<PaymentLinkResponse>.NotFound("Preço não encontrado");
            }

            if (item.UserId != userId)
            {
                return Result<PaymentLinkResponse>.BadRequest("Preço inválido");
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

        await _publishEndpoint.Publish(new PaymentLinkCreatedCommand(
            paymentLink.Id,
            paymentLink.IsActive,
            paymentLink.UserId,
            paymentLink.Items.Select(i =>
                new PaymentLinkItemCreatedCommand(i.Id, i.PaymentLinkId, i.PriceId, i.LiveMode)), 
            paymentLink.LiveMode
        ));

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

    public async Task<Result<PagedResponse<PaymentLinkResponse>>> GetAllPagedAsync(string userId, int page, int limit)
    {
        var result = await _paymentLinkRepository.GetAllPagedAsync(
            userId,
            page,
            limit
        );

        var items = result.Items.Select(pl => new PaymentLinkResponse(
            pl.Id, 
            pl.LiveMode, 
            pl.IsActive, 
            pl.Methods,
            pl.Items.Any() 
                ? pl.Items.Select(item => new PaymentLinkItemResponse(item.Id, item.PriceId, item.Quantity)) 
                : new List<PaymentLinkItemResponse>(), 
            pl.CreatedAt,
            pl.UpdatedAt
        ));
        
        return Result<PagedResponse<PaymentLinkResponse>>.Ok(new PagedResponse<PaymentLinkResponse>(
            items,
            result.Total,
            page,
            (int)Math.Ceiling((double)result.Total / limit),
            limit
        ));
    }
}