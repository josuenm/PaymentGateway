using System.Text.Json;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;
using PaymentLink.Application.PaymentLinks.DTOs.Responses;
using PaymentLink.Application.PaymentLinks.Interfaces;
using PaymentLink.Domain.Entities;
using PaymentLink.Domain.PaymentLinks.Repositories;
using Shared.Kernel.Results;

namespace PaymentLink.Application.PaymentLinks.Services;

public class PaymentLinkService : IPaymentLinkService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IPaymentLinkRepository _paymentLinkRepository;

    public PaymentLinkService(
        IHttpClientFactory httpClientFactory,
        IPaymentLinkRepository paymentLinkRepository 
    )
    {
        _httpClientFactory = httpClientFactory;
        _paymentLinkRepository = paymentLinkRepository;
    }
    
    public async Task<Result<PaymentLinkResponse>> CreateAsync(CreatePaymentLink request, string userId)
    {
        var pricesIdList = request.Items.Select(item => item.PriceId).Distinct().ToList();
        var priceList = string.Join(",", pricesIdList);
        
        var pricesClient = _httpClientFactory.CreateClient("CatalogClient");
        var pricesResponse = await pricesClient.GetAsync($"/api/v1/prices/internal?idList={priceList}");
        var pricesJson = await pricesResponse.Content.ReadAsStringAsync();
        var pricesResult = JsonSerializer.Deserialize<ResultObject<IEnumerable<PriceResponse>>>(
            pricesJson, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        if (pricesResult == null)
            return Result<PaymentLinkResponse>.InternalServerError("Erro ao obter os preços");
            
        if (!pricesResult.Success || pricesResult.Data == null)
            return Result<PaymentLinkResponse>.InternalServerError("Erro ao obter os preços");
        
        var pricesFound = pricesResult.Data.ToList();
        
        if (pricesIdList.Count != pricesFound.Count())
        {
            return Result<PaymentLinkResponse>.BadRequest("Algum preço é inválido");
        }

        foreach (var price in request.Items)
        {
            var item = pricesFound.FirstOrDefault(item => item.Id == price.PriceId);

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

    public async Task<Result<InternalPaymentLinkResponse>> InternalGetByIdAsync(string id)
    {
        var paymentLink = await _paymentLinkRepository.GetByIdAsync(id, true);

        if (paymentLink == null)
        {
            return Result<InternalPaymentLinkResponse>.NotFound("Link de pagamento não encontrado");
        }

        return Result<InternalPaymentLinkResponse>.Ok(new InternalPaymentLinkResponse(
            paymentLink.IsActive, 
            paymentLink.Items.Any()
                ? paymentLink.Items.Select(item => new InternalPaymentLinkItemResponse(item.PriceId, item.Quantity))
                : new List<InternalPaymentLinkItemResponse>()
        ));
    }
}