namespace Payment.Application.Payments.DTOs.Responses;

public abstract record AcquireResponse();

public record CardAcquireResponse(string Id) : AcquireResponse;

public record PixAcquirerResponse(
    string Id, 
    string QrCodeData
) : AcquireResponse;