using PaymentLink.Domain.ProductReadModels.Entities;

namespace PaymentLink.Application.ProductReadModels.Interfaces;

public interface IPriceReadModelService
{
    public Task<IEnumerable<PriceReadModel>> GetManyByIdAsync(IEnumerable<string> idList);
}