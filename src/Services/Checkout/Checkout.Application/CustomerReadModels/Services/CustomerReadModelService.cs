using Checkout.Application.CustomerReadModels.Messaging.Events;
using Checkout.Application.CustomerReadModels.Interfaces;
using Checkout.Domain.CustomerReadModels.Entities;
using Checkout.Domain.CustomerReadModels.Repositories;

namespace Checkout.Application.CustomerReadModels.Services;

public class CustomerReadModelService : ICustomerReadModelService
{
    private readonly ICustomerReadModelRepository _customerReadModelRepository;

    public CustomerReadModelService(ICustomerReadModelRepository customerReadModelRepository)
    {
        _customerReadModelRepository = customerReadModelRepository;
    }

    public async Task CreateAsync(CustomerReadModel customer)
    {
        await _customerReadModelRepository.CreateAsync(customer);
    }
    
    public async Task CreateFromExternalRequestAsync(CustomerCreatedEvent @event)
    {
        var customer = new CustomerReadModel(
            @event.Id,
            @event.Email,
            @event.Name,
            @event.TaxId, 
            @event.UserId, 
            @event.LiveMode
        );

        await _customerReadModelRepository.CreateAsync(customer);
    }

    public async Task<CustomerReadModel?> GetByEmailAndUserIdAsync(string userId, string email)
    {
        return await _customerReadModelRepository.GetByEmailAndUserIdAsync(userId, email);
    }

    public async Task UpdateAsync(CustomerReadModel customer)
    {
        await _customerReadModelRepository.UpdateAsync(customer);
    }
}