using Checkout.Domain.CustomerReadModels.Entities;

namespace Checkout.Domain.CustomerReadModels.Repositories;

public interface ICustomerReadModelRepository
{
    public Task<CustomerReadModel> CreateAsync(CustomerReadModel readModel);
    public Task<CustomerReadModel?> GetByEmailAndUserIdAsync(string userId, string email);
    public Task<CustomerReadModel> UpdateAsync(CustomerReadModel readModel);
}