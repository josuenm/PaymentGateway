using Checkout.Domain.Checkouts.Enums;

namespace Checkout.Application.Checkouts.DTOs.Requests;

public record CreatePaymentRequest(
    PaymentMethod Method,
    string SourceId, 
    string? CustomerId, 
    CustomerPaymentRequest? Customer
);