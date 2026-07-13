using PaymentLink.Application.ProductReadModels.Interfaces;
using PaymentLink.Domain.ProductReadModels.Entities;
using PaymentLink.Domain.ProductReadModels.Repositories;

namespace PaymentLink.Application.ProductReadModels.Services;

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