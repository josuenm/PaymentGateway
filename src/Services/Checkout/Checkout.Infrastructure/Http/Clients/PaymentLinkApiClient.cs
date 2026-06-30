using System.Text.Json;
using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.DTOs.Responses;
using Shared.Kernel.Results;

namespace Checkout.Infrastructure.Http.Clients;

public class PaymentLinkApiClient : IPaymentLinkApiClient
{
    private readonly HttpClient _httpClient;
    
    public PaymentLinkApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<PaymentLinkHttpResponse?> GetAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/v1/paymentlinks/internal/{id}");
        var responseJson = await response.Content.ReadAsStringAsync();
        
        var result = JsonSerializer.Deserialize<ResultObject<PaymentLinkHttpResponse>>(
            responseJson, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (result == null) return null;
        if (!result.Success || result.Data == null) return null;
        
        return result.Data;
    }
}