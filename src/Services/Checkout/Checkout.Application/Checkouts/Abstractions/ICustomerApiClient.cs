using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.DTOs.Responses;

namespace Checkout.Application.Checkouts.Abstractions;

public interface ICustomerApiClient
{
    public Task<CustomerHttpResponse?> GetOrCreateAsync(string userId, CreateCustomerHttpRequest customer);
}