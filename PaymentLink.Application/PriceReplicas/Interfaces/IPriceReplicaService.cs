using PaymentLink.Application.PriceReplicas.DTOs.Requests;
using PaymentLink.Domain.PriceReplicas.Entities;

namespace PaymentLink.Application.PriceReplicas.Interfaces;

public interface IPriceReplicaService
{
    public Task CreateAsync(IEnumerable<CreatePriceReplicaRequest> priceReplica);
}