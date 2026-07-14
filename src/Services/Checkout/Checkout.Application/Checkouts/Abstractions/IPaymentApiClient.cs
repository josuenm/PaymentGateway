using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.DTOs.Responses;

namespace Checkout.Application.Checkouts.Abstractions;

public interface IPaymentApiClient
{
    public Task<PaymentHttpResponse?> CreatePaymentAsync(CreatePaymentHttpRequest request);
    public Task<bool?> ConfirmSandboxPaymentAsync(string paymentId);
}