namespace Checkout.Application.Checkouts.DTOs.Requests;

public record CustomerPaymentHttpRequest(
    string Id, 
    string Email, 
    string Name, 
    string TaxId
);