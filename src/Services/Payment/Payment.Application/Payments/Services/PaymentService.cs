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

    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var isPixMethod = request.Method == PaymentMethod.Pix;
        
        var paymentTransactionId = IdGenerator.Generate("payt");
        var pixQrCodeData = isPixMethod ? PixMock.Generate(paymentTransactionId) : null;
        
        AcquireResponse chargeResponse = isPixMethod
            ? new PixAcquirerResponse(Guid.NewGuid().ToString(), pixQrCodeData!)
            : new CardAcquireResponse(Guid.NewGuid().ToString());
        
        string chargeId = chargeResponse switch
        {
            PixAcquirerResponse pix => pix.Id,
            CardAcquireResponse card => card.Id,
            _ => throw new InvalidOperationException("Tipo de resposta de cobrança inesperado")
        };

        var paymentTransaction = new PaymentTransactionEntity(
            paymentTransactionId, 
            request.Customer.Id, 
            request.Method, 
            isPixMethod ? PaymentStatus.Pending : PaymentStatus.Paid, 
            request.Amount,
            request.Currency,
            request.UserId,
            chargeId, 
            JsonSerializer.Serialize(chargeResponse), 
            true
         );

        await _paymentTransactionRepository.CreateAsync(paymentTransaction);

        return new PaymentResponse(
            paymentTransaction.Id, 
            paymentTransaction.Amount,
            paymentTransaction.Currency,
            paymentTransaction.Status,
            pixQrCodeData 
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