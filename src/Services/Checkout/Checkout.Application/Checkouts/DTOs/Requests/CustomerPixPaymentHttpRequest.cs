namespace Checkout.Application.Checkouts.DTOs.Requests;

public record CustomerPixPaymentHttpRequest(
    string Id, 
    string Email, 
    string Name, 
    string TaxId
);