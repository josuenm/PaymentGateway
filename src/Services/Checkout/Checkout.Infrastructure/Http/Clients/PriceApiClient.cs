using System.Text.Json;
using Checkout.Application.Checkouts.DTOs.Responses;
using Checkout.Infrastructure.Http.Interfaces;
using Shared.Kernel.Results;

namespace Checkout.Infrastructure.Http.Clients;

public class PriceApiClient : IPriceApiClient
{
    private readonly HttpClient _httpClient;
    
    public PriceApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<PriceHttpResponse>?> GetManyByIdAsync(IEnumerable<string> idList)
    {
        var priceList = string.Join(",", idList.ToList());
        
        var response = await _httpClient.GetAsync($"api/v1/prices/internal?idList={priceList}");
        var responseJson = await response.Content.ReadAsStringAsync();
        
        var result = JsonSerializer.Deserialize<ResultObject<IEnumerable<PriceHttpResponse>>>(
            responseJson, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (result == null) return null;
        if (!result.Success || result.Data == null) return null;
        
        return result.Data;
    }
}