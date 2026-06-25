using System.Diagnostics;
using Asp.Versioning;
using Customer.Application.Customers.DTOs.Requests;
using Customer.Application.Customers.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromHeader(Name = "X-User-Id")] string userId, string id)
    {
        var response = await _customerService.GetCustomerByIdAsync(userId, id);
        return response.ToActionResult();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPagedAsync(
        [FromHeader(Name = "X-User-Id")] string userId, 
        [FromQuery] int page = 1, 
        [FromQuery] int limit = 10
    )
    {
        return Ok(await _customerService.GetCustomersPagedAsync(userId, page, limit));
    }
}