namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PaymentHttpResponse (
    string? QrCodeData, 
    string PaymentId, 
    long Amount, 
    string Currency,
    string Status
);