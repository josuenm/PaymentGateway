using PaymentLink.Application.PriceReplicas.DTOs.Requests;
using PaymentLink.Application.PriceReplicas.Interfaces;
using PaymentLink.Domain.PriceReplicas.Entities;
using PaymentLink.Domain.PriceReplicas.Repositories;

namespace PaymentLink.Application.PriceReplicas.Services;

public class PriceReplicaService : IPriceReplicaService
{
    private readonly IPriceReplicaRepository _priceReplicaRepository;

    public PriceReplicaService(IPriceReplicaRepository priceReplicaRepository)
    {
        _priceReplicaRepository = priceReplicaRepository;
    }
    
    public async Task CreateAsync(IEnumerable<CreatePriceReplicaRequest> request)
    {
        var prices = request.Select(price => new PriceReplica(price.Id, price.UserId));
        await _priceReplicaRepository.CreateAsync(prices);
    }
}