namespace Payment.Application.Payments.DTOs.Requests;

public record CardPaymentRequest(
    string Number,
    string ExpiryDate,
    string Cvv,
    string HolderName
);