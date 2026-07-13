using Customer.Application.Customers.Interfaces;
using Customer.Application.Customers.DTOs.Requests;
using Customer.Application.Customers.DTOs.Responses;
using Customer.Application.Customers.Messaging.Events;
using Customer.Application.Customers.Messaging.Commands;
using Customer.Domain.Customers.Entities;
using Customer.Domain.Customers.Repositories;
using MassTransit;
using Shared.DTOs.Responses;
using Shared.Kernel.Results;

namespace Customer.Application.Customers.Services;

public class CustomerService : ICustomerService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository, IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerResponse>> InternalGetOrCreateAsync(
        string userId, 
        CreateCustomerRequest customerRequest
    )
    {
        var customerFound = await _customerRepository.GetByEmailAsync(userId, customerRequest.Email);

        if (customerFound != null)
        {
            return Result<CustomerResponse>.Ok(new CustomerResponse(
                customerFound.Id,
                customerFound.Email,
                customerFound.Name,
                customerFound.TaxId,
                customerFound.CreatedAt
            ));
        }
        
        var newCustomer = new CustomerEntity(
            customerRequest.Email, 
            customerRequest.Name, 
            customerRequest.TaxId, 
            false, 
            userId
        );
        
        await _customerRepository.CreateAsync(newCustomer);
        
        await _publishEndpoint.Publish(new CustomerCreatedEvent(
            newCustomer.Id, 
            newCustomer.Email, 
            newCustomer.Name, 
            newCustomer.TaxId,
            newCustomer.UserId,
            newCustomer.LiveMode
        ));
        
        return Result<CustomerResponse>.Ok(new CustomerResponse(
            newCustomer.Id,
            newCustomer.Email,
            newCustomer.Name,
            newCustomer.TaxId,
            newCustomer.CreatedAt
        ));
    }

    public async Task CreateFromExternalRequestAsync(CreateCustomerCommand @event)
    {
        var customer = CustomerEntity.CreateWithId(
            @event.Id, 
            @event.Email, 
            @event.Name, 
            @event.TaxId, 
            @event.UserId, 
            @event.LiveMode
        );

        await _customerRepository.CreateAsync(customer);
    }

    public async Task UpdateFromExternalRequestAsync(UpdateCustomerCommand command)
    {
        var customer = CustomerEntity.CreateWithId(
            command.Id, 
            command.Email, 
            command.Name, 
            command.TaxId, 
            command.UserId, 
            command.LiveMode
        );
        
        await _customerRepository.UpdateAsync(customer);
    }
    
    public async Task<Result<CustomerResponse>> CreateCustomerAsync(
        string userId, 
        CreateCustomerRequest customerRequest
    )
    {
        var newCustomer = new CustomerEntity(
            customerRequest.Email, 
            customerRequest.Name, 
            customerRequest.TaxId, 
            false, 
            userId
        );
        await _customerRepository.CreateAsync(newCustomer);

        return Result<CustomerResponse>.Created(new CustomerResponse(
            newCustomer.Id,
            newCustomer.Email,
            newCustomer.Name,
            newCustomer.TaxId, 
            newCustomer.CreatedAt
        ));
    }

    public async Task<Result<CustomerResponse>> GetCustomerByIdAsync(string userId, string id)
    {
        var customer = await _customerRepository.GetByIdAsync(userId, id);

        if (customer == null)
            return Result<CustomerResponse>.NotFound("O cliente não existe");

        return Result<CustomerResponse>.Ok(new CustomerResponse(
            customer.Id,
            customer.Email,
            customer.Name,
            customer.TaxId, 
            customer.CreatedAt, 
            customer.UpdatedAt
        ));
    }

    public async Task<PagedResponse<CustomerResponse>> GetCustomersPagedAsync(string userId, int page, int limit)
    {
        var result = await _customerRepository.GetAllAsync(userId, page, limit);

        var items = result.Items.Select(item => 
            new CustomerResponse(
                item.Id,
                item.Email,
                item.Name,
                item.TaxId, 
                item.CreatedAt, 
                item.UpdatedAt
            )
        );

        return new PagedResponse<CustomerResponse>(
            items, 
            result.Total, 
            page, 
            (int)Math.Ceiling((double)result.Total / limit), 
            limit
        );
    }
}