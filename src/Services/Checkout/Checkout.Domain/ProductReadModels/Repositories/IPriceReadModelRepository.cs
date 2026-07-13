using System.Data;
using Checkout.Domain.ProductReadModels.Entities;

namespace Checkout.Domain.ProductReadModels.Repositories;

public interface IPriceReadModelRepository
{
    public Task<IEnumerable<PriceReadModel>> CreateManyAsync(
        IEnumerable<PriceReadModel> prices, 
        IDbTransaction? transaction = null
    );

    public Task<IEnumerable<PriceReadModel>> GetManyByIdAsync(IEnumerable<string> idList);
}