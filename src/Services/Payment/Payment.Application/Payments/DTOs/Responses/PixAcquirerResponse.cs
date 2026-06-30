namespace Payment.Application.Payments.DTOs.Responses;

public record PixAcquirerResponse(
    string Id, 
    string QrCodeData
);