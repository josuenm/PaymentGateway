using Billing.Application.ProductReplica.Interfaces;
using Billing.Application.ProductReplicas.DTOs.Requests;
using Billing.Domain.PriceReplicas;
using Billing.Domain.ProductReplicas;

namespace Billing.Application.ProductReplicas.Services;

public class ProductReplicaService : IProductReplicaService
{
    private readonly IProductReplicaRepository _productRepository;

    public ProductReplicaService(IProductReplicaRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task CreateAsync(CreateProductReplicaRequest request)
    {
        var product = new Domain.ProductReplicas.ProductReplica(
            request.Id, 
            request.Name,
            request.Description,
            request.LiveMode, 
            request.IsActive, 
            request.UserId, 
            request.Metadata
        );

        if (request.Prices != null && request.Prices.Any())
        {
            var prices = request.Prices.Select(price => new PriceReplica(
                price.Id, 
                price.Name, 
                price.AmountInCents, 
                price.Currency, 
                price.ProductReplicaId, 
                price.IsActive,
                price.LiveMode, 
                price.Frequency, 
                price.Cycle
            ));
            product.SetPrices(prices);
        }
        
        await _productRepository.CreateAsync(product);
    }
}