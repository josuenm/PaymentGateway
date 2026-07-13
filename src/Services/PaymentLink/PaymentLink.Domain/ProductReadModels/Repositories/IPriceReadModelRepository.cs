using System.Data;
using PaymentLink.Domain.ProductReadModels.Entities;

namespace PaymentLink.Domain.ProductReadModels.Repositories;

public interface IPriceReadModelRepository
{
    public Task<IEnumerable<PriceReadModel>> CreateManyAsync(
        IEnumerable<PriceReadModel> prices, 
        IDbTransaction? transaction = null
    );

    public Task<IEnumerable<PriceReadModel>> GetManyByIdAsync(IEnumerable<string> idList);
}