using Payment.Application.Payments.DTOs.Requests;
using Payment.Application.Payments.DTOs.Responses;
using Shared.Kernel.Results;

namespace Payment.Application.Payments.Interfaces;

public interface IPaymentService
{
    public Task<PixPaymentResponse> CreatePixPaymentAsync(CreatePixPaymentRequest request);
    public Task<Result<PixPaymentResponse>> GetPixPaymentByIdAsync(string paymentId);
}