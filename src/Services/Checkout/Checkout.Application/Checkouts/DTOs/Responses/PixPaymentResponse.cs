namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PixPaymentResponse(
    string CheckoutId, 
    string PaymentId, 
    string QrCodeData, 
    long ExpiresIn, 
    long Amount
);