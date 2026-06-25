using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Checkout.Application.Checkouts.DTOs.Responses;
using Checkout.Application.Checkouts.Interfaces;
using Shared.Kernel.Results;

namespace Checkout.Application.Checkouts.Services;

public class CheckoutService : ICheckoutService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CheckoutService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<Result<IEnumerable<ItemResponse>>> GetPaymentLinkItemsDetailsAsync(string paymentLinkId)
    {
        try
        {
            var jsonOption = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            var paymentLinkClient = _httpClientFactory.CreateClient("PaymentLinkClient");
            var paymentLinkResponse = await paymentLinkClient.GetAsync($"/api/v1/paymentlink/internal/{paymentLinkId}");
            var paymentLinkJson = await paymentLinkResponse.Content.ReadAsStringAsync();
            var paymentLinkResult = JsonSerializer.Deserialize<ResultObject<PaymentLinkResponse>>(
                paymentLinkJson, 
                jsonOption
            );

            if (paymentLinkResult == null)
                return Result<IEnumerable<ItemResponse>>.InternalServerError("Erro ao obter o link de pagamento");
            
            if (!paymentLinkResult.Success && paymentLinkResponse.StatusCode == HttpStatusCode.NotFound)
                return Result<IEnumerable<ItemResponse>>.NotFound("O link de pagamento não existe");
            
            if (!paymentLinkResult.Success)
                return Result<IEnumerable<ItemResponse>>.InternalServerError("Erro ao obter o link de pagamento");

            var paymentLink = paymentLinkResult.Data;
            
            if (paymentLink == null)
                return Result<IEnumerable<ItemResponse>>.NotFound("O link de pagamento não existe");
            
            if (!paymentLink.IsActive)
                return Result<IEnumerable<ItemResponse>>.NotFound("O link de pagamento não existe");

            var paymentLinkItems = paymentLink.Items.ToList();
            
            var priceIdList = paymentLinkItems
                .Select(p => p.PriceId)
                .ToList();
            
            var priceList = string.Join(",", priceIdList);
            
            var pricesClient = _httpClientFactory.CreateClient("CatalogClient");
            var pricesResponse = await pricesClient.GetAsync($"/api/v1/prices/internal?idList={priceList}");
            var pricesJson = await pricesResponse.Content.ReadAsStringAsync();
            var pricesResult = JsonSerializer.Deserialize<ResultObject<IEnumerable<PriceResponse>>>(
                pricesJson, 
                jsonOption
            );
            
            if (pricesResult == null)
                return Result<IEnumerable<ItemResponse>>.InternalServerError("Erro ao obter itens");
            
            if (!pricesResult.Success || pricesResult.Data == null)
                return Result<IEnumerable<ItemResponse>>.InternalServerError("Erro ao obter itens");

            var items = pricesResult.Data.Select(p =>
            {
                var item = paymentLinkItems.FirstOrDefault(i =>
                {
                    Console.WriteLine($"PAYMENT LINK ITEM ID: {i.PriceId} | PRICE ID: {p.Id}");
                    return i.PriceId == p.Id;
                });

                if (item == null) return null;
                
                return new ItemResponse(
                    p.ProductId,
                    p.Id,
                    p.Name,
                    item.Quantity,
                    p.Currency,
                    p.Amount, 
                    p.Frequency,
                    p.Cycle
                );
            })
            .Where(p => p != null)
            .Select(p => p!)
            .ToList();
            
            return Result<IEnumerable<ItemResponse>>.Ok(items);
        }
        catch (HttpRequestException e)
        {
            return Result<IEnumerable<ItemResponse>>.BadRequest("Erro ao obter o link de pagamento");
        }
    }
}