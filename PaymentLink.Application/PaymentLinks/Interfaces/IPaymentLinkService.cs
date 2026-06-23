using PaymentLink.Application.PaymentLinks.DTOs.Requests;
using PaymentLink.Application.PaymentLinks.DTOs.Responses;
using Shared.Kernel.Results;

namespace PaymentLink.Application.PaymentLinks.Interfaces;

public interface IPaymentLinkService
{
    public Task<Result<PaymentLinkResponse>> CreateAsync(CreatePaymentLink request, string userId);
    public Task<Result<InternalPaymentLinkResponse>> GetInternalByIdAsync(string id);
}