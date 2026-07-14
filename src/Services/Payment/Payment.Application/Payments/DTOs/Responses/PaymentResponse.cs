using Payment.Domain.Payments.Enums;

namespace Payment.Application.Payments.DTOs.Responses;

public record PaymentResponse(
    string PaymentId, 
    long Amount, 
    string Currency,
    PaymentStatus Status,
    string? QrCodeData = null 
);