namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PaymentResponse(
    string CheckoutId, 
    string PaymentId, 
    string? QrCodeData = null, 
    long? ExpiresIn = null, 
    long? Amount = null
);