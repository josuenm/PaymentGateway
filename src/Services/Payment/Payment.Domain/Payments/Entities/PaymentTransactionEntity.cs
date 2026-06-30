using Payment.Domain.Payments.Enums;
using Shared.Kernel.Utils;

namespace Payment.Domain.Payments.Entities;

public class PaymentTransactionEntity
{
    public string Id { get; private set; }
    public string CustomerId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public long Amount { get; private set; }
    public string Currency { get; private set; }
    public string UserId { get; private set; }
    public bool LiveMode { get; private set; }
    public string ChargeId { get; private set; }
    public object ChargeResponse { get; private set; }
    public DateTime CreadtedAt { get; private set; }    
    public DateTime? PaidAt { get; private set; } 

    public PaymentTransactionEntity(
        string customerId, 
        PaymentMethod method, 
        PaymentStatus status, 
        long amount, 
        string currency, 
        string userId,
        string chargeId,
        object chargeResponse,
        bool liveMode 
    )
    {
        Id = IdGenerator.Generate("payt");
        CustomerId = customerId;
        Method = method;
        Status = status;
        Amount = amount;
        Currency = currency;
        UserId = userId;
        ChargeId = chargeId;
        ChargeResponse = chargeResponse;
        LiveMode = liveMode;
        CreadtedAt = DateTime.UtcNow;
        PaidAt = null;
    }
    
    public PaymentTransactionEntity(
        string id, 
        string customerId, 
        PaymentMethod method, 
        PaymentStatus status, 
        long amount, 
        string currency, 
        string userId,
        string chargeId,
        object chargeResponse,
        bool liveMode 
    )
    {
        Id = id;
        CustomerId = customerId;
        Method = method;
        Status = status;
        Amount = amount;
        Currency = currency;
        UserId = userId;
        LiveMode = liveMode;
        ChargeId = chargeId;
        ChargeResponse = chargeResponse;
        CreadtedAt = DateTime.UtcNow;
        PaidAt = null;
    }

    public void SetPaid()
    {
        Status = PaymentStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }
    
    private PaymentTransactionEntity() { }
}