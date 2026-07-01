using Customer.API.Controllers;
using Customer.Application.Customers.DTOs.Requests;
using Customer.Application.Customers.DTOs.Responses;
using Customer.Application.Customers.Interfaces;
using Customer.Application.Customers.Validators;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Kernel.Results;

namespace Customer.Test.Controllers;

public class CustomersControllerTest
{
    private readonly Mock<ICustomerService> _customerServiceMock;
    private readonly CustomersController _customersController;

    public CustomersControllerTest()
    {
        _customerServiceMock = new Mock<ICustomerService>();
        _customersController = new CustomersController(
            _customerServiceMock.Object,
            new CreateCustomerRequestValidator()
        );
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ReturnsBadRequest()
    {
        var request = new CreateCustomerRequest("", "John Doe", "12345678901");

        var result = await _customersController.CreateAsync("usr_123", request);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(objectResult.Value);

        Assert.Equal(400, objectResult.StatusCode);
        Assert.False(resultValue.Success);

        Assert.NotNull(resultValue.Error);
        Assert.NotNull(resultValue.Error.Details);

        Assert.Equal("1 ou mais campos inválidos", resultValue.Error.Message);
        Assert.Contains("Email", resultValue.Error.Details.Keys);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreated()
    {
        var request = new CreateCustomerRequest("example@example.com", "John Doe", "12345678901");

        _customerServiceMock
            .Setup(service => service.CreateCustomerAsync("usr_123", request))
            .ReturnsAsync(Result<CustomerResponse>.Created(new CustomerResponse(
                "cust_123",
                request.Email,
                request.Name,
                request.TaxId,
                DateTime.UtcNow
            )));

        var result = await _customersController.CreateAsync("usr_123", request);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<CustomerResponse>>(objectResult.Value);

        Assert.Equal(201, objectResult.StatusCode);
        Assert.True(resultValue.Success);

        Assert.NotNull(resultValue.Data);
        Assert.Null(resultValue.Error);
        Assert.Equal(request.Email, resultValue.Data.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOk()
    {
        var userId = "usr_123";
        var customerId = "cust_123";

        var customerResponse = new CustomerResponse(
            customerId,
            "example@example.com",
            "John Doe",
            "12345678901",
            DateTime.UtcNow
        );

        _customerServiceMock
            .Setup(service => service.GetCustomerByIdAsync(userId, customerId))
            .ReturnsAsync(Result<CustomerResponse>.Ok(customerResponse));

        var result = await _customersController.GetByIdAsync(userId, customerId);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<CustomerResponse>>(objectResult.Value);

        Assert.Equal(200, objectResult.StatusCode);
        Assert.True(resultValue.Success);

        Assert.NotNull(resultValue.Data);
        Assert.Null(resultValue.Error);
        Assert.Equal(customerResponse.Id, resultValue.Data.Id);
    }

    [Fact]
    public async Task GetAllPagedAsync_ReturnsOk()
    {
        var userId = "usr_123";
        var page = 1;
        var limit = 10;

        _customerServiceMock
            .Setup(service => service.GetCustomersPagedAsync(userId, page, limit))
            .ReturnsAsync(new PagedResponse<CustomerResponse>(
                new List<CustomerResponse>(),
                0,
                page,
                1,
                limit
            ));

        var result = await _customersController.GetAllPagedAsync(userId, page, limit);
        var resultObject = Assert.IsType<OkObjectResult>(result);
        var resultValue = Assert.IsType<PagedResponse<CustomerResponse>>(resultObject.Value);

        Assert.Equal(200, resultObject.StatusCode);
        Assert.Equal(page, resultValue.Page);
        Assert.Equal(limit, resultValue.Limit);
    }

    [Fact]
    public async Task InternalGetOrCreateAsync_WithInvalidData_ReturnsBadRequest()
    {
        var request = new CreateCustomerRequest("", "John Doe", "12345678901");

        var result = await _customersController.InternalGetOrCreateAsync("usr_123", request);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(objectResult.Value);

        Assert.Equal(400, objectResult.StatusCode);
        Assert.False(resultValue.Success);

        Assert.NotNull(resultValue.Error);
        Assert.NotNull(resultValue.Error.Details);

        Assert.Equal("1 ou mais campos inválidos", resultValue.Error.Message);
        Assert.Contains("Email", resultValue.Error.Details.Keys);
    }
}
