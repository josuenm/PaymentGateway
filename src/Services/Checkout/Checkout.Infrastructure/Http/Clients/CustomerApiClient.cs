using System.Text;
using System.Text.Json;
using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.DTOs.Responses;
using Shared.Kernel.Results;

namespace Checkout.Infrastructure.Http.Clients;

public class CustomerApiClient : ICustomerApiClient
{
    private readonly HttpClient _httpClient;
    
    public CustomerApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CustomerHttpResponse?> GetOrCreateAsync(string userId, CreateCustomerHttpRequest customer)
    {
        var json = JsonSerializer.Serialize(new CreateCustomerHttpRequest
        (
            customer.Email!,
            customer?.Name,
            customer?.TaxId
        ));
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customers/internal/get-or-create")
        {
            Content = content
        };
        
        request.Headers.Add("X-User-Id", userId);
        
        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();
        
        var result = JsonSerializer.Deserialize<ResultObject<CustomerHttpResponse>>(
            responseJson, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        if (result == null) return null;
        if (!result.Success || result.Data == null) return null;
        
        return result.Data;
    }
}