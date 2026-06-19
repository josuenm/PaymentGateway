using Customers.Application.Customers.Entities;
using Customers.Domain.Commons;

namespace Customers.Domain.Customers.Repositories;

public interface ICustomerRepository
{
    public Task<Customer> CreateAsync(Customer customer);
    public Task<Customer?> GetByIdAsync(string userId, string id);
    public Task<PagedSearchResult<Customer>> GetAllAsync(string userId, int page, int limit);
}