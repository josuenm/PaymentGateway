using System.Text;
using System.Text.Json;
using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.DTOs.Responses;
using Shared.Kernel.Results;

namespace Checkout.Infrastructure.Http.Clients;

public class PaymentApiClient : IPaymentApiClient
{
    private readonly HttpClient _httpClient;
    
    public PaymentApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<PaymentHttpResponse?> CreatePaymentAsync(CreatePaymentHttpRequest pixPayment)
    {
        var json = JsonSerializer.Serialize(pixPayment);
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/payments/internal/pix")
        {
            Content = content
        };

        request.Headers.Add("X-User-Id", pixPayment.UserId);
        
        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ResultObject<PaymentHttpResponse>>(
            responseJson, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (result == null) return null;
        if (!result.Success && result.Data == null) return null;

        return result.Data;
    }

    public async Task<bool?> ConfirmSandboxPaymentAsync(string paymentId)
    {
        var response = await _httpClient.GetAsync($"api/v1/payments/internal/confirm/sandbox/{paymentId}");
        var responseJson = await response.Content.ReadAsStringAsync();
        
        var result = JsonSerializer.Deserialize<ResultObject<bool?>>(
            responseJson, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (result == null) return null;
        if (!result.Success || result.Data == null) return null;
        
        return result.Data;
    }
}