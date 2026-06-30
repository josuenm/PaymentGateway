using Payment.Domain.Payments.Enums;

namespace Payment.Application.Payments.DTOs.Requests;

public record CreatePixPaymentRequest(
    CustomerRequest Customer, 
    PaymentMethod Method,
    long Amount, 
    string Currency,
    string UserId, 
    bool LiveMode
);