using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.DTOs.Responses;
using Shared.Kernel.Results;

namespace Checkout.Application.Checkouts.Interfaces;

public interface ICheckoutService
{
    public Task<Result<IEnumerable<ItemResponse>>> GetPaymentLinkItemsDetailsAsync(string paymentLinkId);
    public Task<Result<PixPaymentResponse>> CreatePixPaymentAsync(CreatePaymentRequest paymentRequest);
}