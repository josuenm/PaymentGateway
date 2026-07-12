using System.Data;
using Catalog.Domain.Commons;
using Catalog.Domain.Prices.Entities;

namespace Catalog.Domain.Prices.Repositories;

public interface IPriceRepository
{
    public Task<Price> CreateAsync(Price price);
    public Task<IEnumerable<Price>> CreateManyAsync(
        IEnumerable<Price> prices, 
        IDbTransaction? transaction = null 
    );
    public Task<PagedSearchResult<Price>> GetAllPagedAsync(string userId, int page, int limit);
    public Task<IEnumerable<Price>> GetManyByIdAsync(
        IEnumerable<string> idList, 
        bool? priceActive = true, 
        bool? productActive = true
    );
}