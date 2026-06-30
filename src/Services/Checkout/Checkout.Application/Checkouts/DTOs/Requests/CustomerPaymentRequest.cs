namespace Checkout.Application.Checkouts.DTOs.Requests;

public record CustomerPaymentRequest(
    string Name, 
    string Email, 
    string TaxId,
    string? Phone 
);