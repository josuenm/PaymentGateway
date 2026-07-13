using Checkout.Domain.ProductReadModels.Entities;

namespace Checkout.Application.ProductReadModels.Interfaces;

public interface IPriceReadModelService
{
    public Task<IEnumerable<PriceReadModel>> GetManyByIdAsync(IEnumerable<string> idList);
}