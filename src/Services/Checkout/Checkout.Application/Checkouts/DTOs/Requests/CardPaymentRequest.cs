namespace Checkout.Application.Checkouts.DTOs.Requests;

public record CardPaymentRequest(
    string Number,
    string ExpiryDate,
    string Cvv,
    string HolderName
);