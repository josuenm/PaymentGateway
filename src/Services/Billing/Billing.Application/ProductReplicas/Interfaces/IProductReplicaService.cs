using Billing.Application.ProductReplicas.DTOs.Requests;

namespace Billing.Application.ProductReplica.Interfaces;

public interface IProductReplicaService
{
    public Task CreateAsync(CreateProductReplicaRequest request);
}