using Catalog.Domain.Commons;
using Catalog.Domain.Products.Entities;

namespace Catalog.Domain.Products.Repositories;

public interface IProductRepository
{
    public Task<Product> CreateAsync(Product product);
    public Task<Product?> GetByIdAsync(string userId, string id);
    public Task<Product?> GetByNameAsync(string userId, string name);
    public Task<PagedSearchResult<Product>> GetAllPagedAsync(string userId, int page, int limit);
}