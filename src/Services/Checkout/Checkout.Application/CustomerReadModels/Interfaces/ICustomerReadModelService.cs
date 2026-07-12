using Checkout.Application.CustomerReadModels.Messaging.Events;
using Checkout.Domain.CustomerReadModels.Entities;

namespace Checkout.Application.CustomerReadModels.Interfaces;

public interface ICustomerReadModelService
{
    public Task CreateAsync(CustomerReadModel customer);
    public Task CreateFromExternalRequestAsync(CustomerCreatedEvent @event);
    public Task<CustomerReadModel?> GetByEmailAndUserIdAsync(string userId, string email);
    public Task UpdateAsync(CustomerReadModel customer);
}