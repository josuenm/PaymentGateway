using Customer.Application.Customers.DTOs.Requests;
using Customer.Application.Customers.DTOs.Responses;
using Customer.Application.Customers.Messaging.Events;
using Shared.DTOs.Responses;
using Shared.Kernel.Results;

namespace Customer.Application.Customers.Interfaces;

public interface ICustomerService
{
    public Task CreateFromExternalRequestAsync(CreateCustomerCommand @event);
    public Task UpdateFromExternalRequestAsync(UpdateCustomerCommand command);
    public Task<Result<CustomerResponse>> CreateCustomerAsync(string userId, CreateCustomerRequest customerRequest);
    public Task<Result<CustomerResponse>> InternalGetOrCreateAsync(string userId, CreateCustomerRequest customer);
    public Task<Result<CustomerResponse>> GetCustomerByIdAsync(string userId, string id);
    public Task<PagedResponse<CustomerResponse>> GetCustomersPagedAsync(string userId, int page, int limit);
}