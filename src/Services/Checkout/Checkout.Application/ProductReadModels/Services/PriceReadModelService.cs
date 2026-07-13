using Checkout.Application.ProductReadModels.Interfaces;
using Checkout.Domain.ProductReadModels.Entities;
using Checkout.Domain.ProductReadModels.Repositories;

namespace Checkout.Application.ProductReadModels.Services;

public class PriceReadModelService : IPriceReadModelService
{
    private readonly IPriceReadModelRepository _priceReadModelRepository;

    public PriceReadModelService(IPriceReadModelRepository priceReadModelRepository)
    {
        _priceReadModelRepository = priceReadModelRepository;
    }
    
    public async Task<IEnumerable<PriceReadModel>> GetManyByIdAsync(IEnumerable<string> idList)
    {
        return await  _priceReadModelRepository.GetManyByIdAsync(idList);        
    }
}