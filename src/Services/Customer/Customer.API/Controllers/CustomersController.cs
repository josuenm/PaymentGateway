using System.Diagnostics;
using Asp.Versioning;
using Customer.Application.Customers.DTOs.Requests;
using Customer.Application.Customers.DTOs.Responses;
using Customer.Application.Customers.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Responses;
using Shared.Infrastructure.Attributes;
using Shared.Kernel.Results;

namespace Customer.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    private readonly IValidator<CreateCustomerRequest> _createCustomerRequestValidator;

    public CustomersController(
        ICustomerService customerService, 
        IValidator<CreateCustomerRequest> createCustomerRequestValidator
    )
    {
        _customerService = customerService;
        _createCustomerRequestValidator = createCustomerRequestValidator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResultObject<CustomerResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultObject<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromHeader(Name = "X-User-Id")] string userId, 
        [FromBody] CreateCustomerRequest request
    )
    {
        var validationResult = await _createCustomerRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(e => e.ErrorMessage)
                );
            
            return Result<object>
                .BadRequest("1 ou mais campos inválidos", errors, Activity.Current?.Id)
                .ToActionResult();
        }
        
        var result = await _customerService.CreateCustomerAsync(userId, request);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResultObject<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultObject<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromHeader(Name = "X-User-Id")] string userId, string id)
    {
        var response = await _customerService.GetCustomerByIdAsync(userId, id);
        return response.ToActionResult();
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResultObject<PagedResponse<CustomerResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPagedAsync(
        [FromHeader(Name = "X-User-Id")] string userId, 
        [FromQuery] int page = 1, 
        [FromQuery] int limit = 10
    )
    {
        return Ok(await _customerService.GetCustomersPagedAsync(userId, page, limit));
    }
}