using Billing.Domain.PriceReplicas.Enums;

namespace Billing.Application.PriceReplicas.DTOs.Requests;

public record CreatePriceReplicaRequest(
    string Id, 
    string Name, 
    long AmountInCents, 
    string Currency, 
    string ProductReplicaId, 
    bool IsActive,
    bool LiveMode, 
    PriceReplicaFrequency Frequency, 
    PriceReplicaCycle? Cycle
);