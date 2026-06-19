using Customers.Application.Customers.DTOs.Requests;
using Customers.Application.Customers.DTOs.Responses;
using Shared.Kernel.Results;

namespace Customers.Application.Customers.Interfaces;

public interface ICustomerService
{
    public Task<Result<CustomerResponse>> CreateCustomerAsync(string userId, CreateCustomerRequest customerRequest);
    public Task<Result<CustomerResponse>> GetCustomerByIdAsync(string userId, string id);
    public Task<PagedResponse<CustomerResponse>> GetCustomersPagedAsync(string userId, int page, int limit);
}