using Checkout.Application.Checkouts.DTOs.Responses;

namespace Checkout.Application.Checkouts.Abstractions;

public interface IPaymentLinkApiClient
{
    public Task<PaymentLinkHttpResponse?> GetAsync(string id);
}