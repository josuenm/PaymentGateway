using Customers.Application.Customers.Interfaces;
using Customers.Application.Customers.DTOs.Requests;
using Customers.Application.Customers.DTOs.Responses;
using Customers.Application.Customers.Entities;
using Customers.Domain.Customers.Repositories;
using Shared.Kernel.Results;

namespace Customers.Application.Customers.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    
    public async Task<Result<CustomerResponse>> CreateCustomerAsync(
        string userId, 
        CreateCustomerRequest customerRequest
    )
    {
        var newCustomer = new Customer(
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
            return Result<CustomerResponse>.Failure("O cliente não existe", ErrorType.NotFound);

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