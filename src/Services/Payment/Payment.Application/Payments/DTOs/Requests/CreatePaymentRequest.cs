using Payment.Domain.Payments.Enums;

namespace Payment.Application.Payments.DTOs.Requests;

public record CreatePaymentRequest(
    CustomerRequest Customer, 
    PaymentMethod Method,
    long Amount, 
    string Currency,
    string UserId, 
    bool LiveMode,
    CardPaymentRequest? Card = null
);