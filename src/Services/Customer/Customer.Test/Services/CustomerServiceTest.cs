using Customer.Application.Customers.DTOs.Requests;
using Customer.Application.Customers.DTOs.Responses;
using Customer.Application.Customers.Services;
using Customer.Domain.Commons;
using Customer.Domain.Customers.Entities;
using Customer.Domain.Customers.Repositories;
using Moq;
using Shared.Kernel.Results;

namespace Customer.Test.Services;

public class CustomerServiceTest
{
    private readonly CustomerService _customerService;
    private readonly Mock<ICustomerRepository> _customerRepository;

    public CustomerServiceTest()
    {
        _customerRepository = new Mock<ICustomerRepository>();
        _customerService = new CustomerService(_customerRepository.Object);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithValidData_ReturnsCreated()
    {
        var userId = "usr_123";
        var request = new CreateCustomerRequest("example@example.com", "John Doe", "12345678901");

        _customerRepository
            .Setup(method => method.CreateAsync(It.IsAny<CustomerEntity>()))
            .ReturnsAsync((CustomerEntity customer) => customer);

        var result = await _customerService.CreateCustomerAsync(userId, request);
        var resultObject = Assert.IsType<Result<CustomerResponse>>(result);

        Assert.Equal(201, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
        Assert.Equal(request.Email, resultObject.Data.Email);
        Assert.Equal(request.Name, resultObject.Data.Name);
        Assert.Equal(request.TaxId, resultObject.Data.TaxId);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WithInvalidId_ReturnsNotFound()
    {
        var userId = "usr_123";
        var customerId = "cust_123";

        _customerRepository
            .Setup(method => method.GetByIdAsync(userId, customerId))
            .ReturnsAsync((CustomerEntity?)null);

        var result = await _customerService.GetCustomerByIdAsync(userId, customerId);
        var resultObject = Assert.IsType<Result<CustomerResponse>>(result);

        Assert.Equal(404, resultObject.StatusCode);
        Assert.Null(resultObject.Data);
        Assert.NotNull(resultObject.Error);
        Assert.False(resultObject.Success);
        Assert.Equal("O cliente não existe", resultObject.Error.Message);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WithValidId_ReturnsOk()
    {
        var userId = "usr_123";
        var customer = new CustomerEntity("example@example.com", "John Doe", "12345678901", false, userId);

        _customerRepository
            .Setup(method => method.GetByIdAsync(userId, customer.Id))
            .ReturnsAsync(customer);

        var result = await _customerService.GetCustomerByIdAsync(userId, customer.Id);
        var resultObject = Assert.IsType<Result<CustomerResponse>>(result);

        Assert.Equal(200, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
        Assert.Equal(customer.Id, resultObject.Data.Id);
        Assert.Equal(customer.Email, resultObject.Data.Email);
    }

    [Fact]
    public async Task InternalGetOrCreateAsync_WithExistingCustomer_ReturnsOk()
    {
        var userId = "usr_123";
        var request = new CreateCustomerRequest("example@example.com", "John Doe", "12345678901");
        var existingCustomer = new CustomerEntity(request.Email, request.Name, request.TaxId, false, userId);

        _customerRepository
            .Setup(method => method.GetByEmailAsync(userId, request.Email))
            .ReturnsAsync(existingCustomer);

        var result = await _customerService.InternalGetOrCreateAsync(userId, request);
        var resultObject = Assert.IsType<Result<CustomerResponse>>(result);

        Assert.Equal(200, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
        Assert.Equal(existingCustomer.Id, resultObject.Data.Id);
    }

    [Fact]
    public async Task InternalGetOrCreateAsync_WithNewCustomer_ReturnsOk()
    {
        var userId = "usr_123";
        var request = new CreateCustomerRequest("example@example.com", "John Doe", "12345678901");

        _customerRepository
            .Setup(method => method.GetByEmailAsync(userId, request.Email))
            .ReturnsAsync((CustomerEntity?)null);

        _customerRepository
            .Setup(method => method.CreateAsync(It.IsAny<CustomerEntity>()))
            .ReturnsAsync((CustomerEntity customer) => customer);

        var result = await _customerService.InternalGetOrCreateAsync(userId, request);
        var resultObject = Assert.IsType<Result<CustomerResponse>>(result);

        Assert.Equal(200, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
        Assert.Equal(request.Email, resultObject.Data.Email);
    }

    [Fact]
    public async Task GetCustomersPagedAsync_ReturnsPagedResponse()
    {
        var userId = "usr_123";
        var page = 1;
        var limit = 10;
        var customer = new CustomerEntity("example@example.com", "John Doe", "12345678901", false, userId);

        _customerRepository
            .Setup(method => method.GetAllAsync(userId, page, limit))
            .ReturnsAsync(new PagedSearchResult<CustomerEntity>(new List<CustomerEntity> { customer }, 1));

        var result = await _customerService.GetCustomersPagedAsync(userId, page, limit);

        Assert.Equal(page, result.Page);
        Assert.Equal(limit, result.Limit);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }
}
