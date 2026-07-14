using Payment.Application.Payments.DTOs.Requests;
using Payment.Application.Payments.DTOs.Responses;
using Shared.Kernel.Results;

namespace Payment.Application.Payments.Interfaces;

public interface IPaymentService
{
    public Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
    public Task<Result<bool>> ConfirmSandboxPaymentAsync(string paymentId);
}