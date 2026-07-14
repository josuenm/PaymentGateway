using Checkout.Domain.Checkouts.Enums;

namespace Checkout.Application.Checkouts.DTOs.Requests;

public record CreatePaymentHttpRequest(
    CustomerPaymentHttpRequest Customer, 
    PaymentMethod Method,
    long Amount, 
    string Currency,
    string UserId, 
    bool LiveMode
);