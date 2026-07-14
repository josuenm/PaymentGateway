using System.Text.Json.Serialization;

namespace Checkout.Application.Checkouts.DTOs.Responses;

public record PaymentResponse(
    string CheckoutId, 
    string PaymentId, 
    string Status,
    
    long Amount,
        
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? QrCodeData = null,
    
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    long? ExpiresIn = null
);