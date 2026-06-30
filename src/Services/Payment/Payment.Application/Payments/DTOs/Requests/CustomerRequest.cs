namespace Payment.Application.Payments.DTOs.Requests;

public record CustomerRequest(
    string Id, 
    string Email, 
    string Name, 
    string TaxId
);