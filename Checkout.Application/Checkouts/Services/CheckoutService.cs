using System.Net.Http.Json;
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
            var client = _httpClientFactory.CreateClient("PaymentLinkClient");
            var paymentLink = await client
                .GetFromJsonAsync<PaymentLinkResponse?>("/api/v1/internal/{paymentLinkId}");

            if (paymentLink == null)
            {
                return Result<IEnumerable<ItemResponse>>.NotFound("Link de pagamento não encontrado");
            }

            if (!paymentLink.IsActive)
            {
                return Result<IEnumerable<ItemResponse>>.NotFound("O link de pagamento não esta mais ativo");
            }
        }
        catch (HttpRequestException e)
        {
            return Result<IEnumerable<ItemResponse>>.BadRequest("Erro ao obter o link de pagamento");
        }

        throw new NotImplementedException();
    }
}