namespace PaymentLink.Application.PriceReplicas.DTOs.Requests;

public record CreatePriceReplicaRequest(
    string Id, 
    string UserId
);