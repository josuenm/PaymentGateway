using Payment.Domain.Payments.Enums;

namespace Payment.Application.Payments.DTOs.Responses;

public record PixPaymentResponse(
    string QrCodeData, 
    string PaymentId, 
    long Amount, 
    string Currency,
    PaymentStatus Status
);