using System.Diagnostics;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Payments.DTOs.Requests;
using Payment.Application.Payments.Interfaces;
using Shared.Infrastructure.Attributes;
using Shared.Kernel.Results;

namespace Payment.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IValidator<CreatePixPaymentRequest> _createPixPaymentRequestValidator;

    public PaymentsController(
        IPaymentService paymentService, 
        IValidator<CreatePixPaymentRequest> createPixPaymentRequestValidator
    )
    {
        _paymentService = paymentService;
        _createPixPaymentRequestValidator = createPixPaymentRequestValidator;
    }

    [HttpPost("internal/pix/create")]
    [InternalAuthorize]
    public async Task<IActionResult> CreatePixPaymentAsync([FromBody] CreatePixPaymentRequest request)
    {
        var validationResult = await _createPixPaymentRequestValidator.ValidateAsync(request);

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
        
        var result = await _paymentService.CreatePixPaymentAsync(request);
        return Result<object>.Created(result).ToActionResult();
    }

    [HttpGet("internal/{id}")]
    [InternalAuthorize]
    public async Task<IActionResult> GetPaymentByIdAsync(string id)
    {
        return (await _paymentService.GetPixPaymentByIdAsync(id)).ToActionResult();
    }
}