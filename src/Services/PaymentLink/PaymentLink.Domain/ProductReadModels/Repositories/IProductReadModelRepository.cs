using PaymentLink.Domain.ProductReadModels.Entities;

namespace PaymentLink.Domain.ProductReadModels.Repositories;

public interface IProductReadModelRepository
{
    public Task<ProductReadModel> CreateAsync(ProductReadModel readModel);
}