namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PixPaymentHttpResponse (
    string QrCodeData, 
    string PaymentId, 
    long Amount, 
    string Currency,
    string Status
);