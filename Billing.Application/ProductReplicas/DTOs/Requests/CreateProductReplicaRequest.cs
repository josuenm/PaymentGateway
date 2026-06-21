using Billing.Application.PriceReplicas.DTOs.Requests;

namespace Billing.Application.ProductReplicas.DTOs.Requests;

public record CreateProductReplicaRequest(
    string Id, 
    string Name, 
    string? Description, 
    bool LiveMode, 
    bool IsActive, 
    string UserId, 
    IEnumerable<CreatePriceReplicaRequest>? Prices, 
    object? Metadata
);