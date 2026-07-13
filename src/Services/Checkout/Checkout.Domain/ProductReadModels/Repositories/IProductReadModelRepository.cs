using Checkout.Domain.ProductReadModels.Entities;

namespace Checkout.Domain.ProductReadModels.Repositories;

public interface IProductReadModelRepository
{
    public Task<ProductReadModel> CreateAsync(ProductReadModel readModel);
}