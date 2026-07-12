using Customer.Domain.Commons;
using Customer.Domain.Customers.Entities;

namespace Customer.Domain.Customers.Repositories;

public interface ICustomerRepository
{
    public Task<CustomerEntity> CreateAsync(CustomerEntity customer);
    public Task<CustomerEntity> UpdateAsync(CustomerEntity customer);
    public Task<CustomerEntity?> GetByEmailAsync(string userId, string email);
    public Task<CustomerEntity?> GetByIdAsync(string userId, string id);
    public Task<PagedSearchResult<CustomerEntity>> GetAllAsync(string userId, int page, int limit);
}