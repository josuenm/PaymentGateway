using System.Text.Json;
using Payment.Application.Payments.DTOs.Requests;
using Payment.Application.Payments.DTOs.Responses;
using Payment.Application.Payments.Interfaces;
using Payment.Domain.Payments.Entities;
using Payment.Domain.Payments.Enums;
using Payment.Domain.Payments.Repositories;
using Shared.Kernel.Results;
using Shared.Kernel.Utils;
using Shared.Mocks;

namespace Payment.Application.Payments.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentTransactionRepository _paymentTransactionRepository;

    public PaymentService(IPaymentTransactionRepository paymentTransactionRepository)
    {
        _paymentTransactionRepository = paymentTransactionRepository;
    }

    public async Task<PixPaymentResponse> CreatePixPaymentAsync(CreatePixPaymentRequest request)
    {
        var paymentTransactionId = IdGenerator.Generate("payt");
        var pixQrCodeData = PixMock.Generate(paymentTransactionId);
        var chargeResponse = new PixAcquirerResponse
        (
            Guid.NewGuid().ToString(),
            pixQrCodeData
        );
        
        var paymentTransaction = new PaymentTransactionEntity(
            paymentTransactionId, 
            request.Customer.Id, 
            request.Method, 
            PaymentStatus.Pending, 
            request.Amount,
            request.Currency,
            request.UserId,
            chargeResponse.Id, 
            JsonSerializer.Serialize(chargeResponse), 
            true
         );

        await _paymentTransactionRepository.CreateAsync(paymentTransaction);

        return new PixPaymentResponse(
            pixQrCodeData, 
            paymentTransaction.Id, 
            paymentTransaction.Amount,
            paymentTransaction.Currency,
            paymentTransaction.Status
        );
    }

    public async Task<Result<bool>> ConfirmSandboxPaymentAsync(string paymentId)
    {
        var payment = await _paymentTransactionRepository.GetByIdAsync(paymentId);
        
        if (payment == null)
            return Result<bool>.NotFound("Pagamento não encontrado");

        if (!payment.LiveMode)
            return Result<bool>.BadRequest("O pagamento não é sandbox");

        if (payment.Status == PaymentStatus.Paid)
            return Result<bool>.Ok(true);
        
        payment.SetPaid();
        await _paymentTransactionRepository.SetPaidAsync(payment);
        
        return Result<bool>.Ok(payment.Status == PaymentStatus.Paid);
    }
}