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
    private readonly IValidator<CreatePaymentRequest> _createPixPaymentRequestValidator;

    public PaymentsController(
        IPaymentService paymentService, 
        IValidator<CreatePaymentRequest> createPixPaymentRequestValidator
    )
    {
        _paymentService = paymentService;
        _createPixPaymentRequestValidator = createPixPaymentRequestValidator;
    }

    [HttpPost("internal/pix")]
    [InternalAuthorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> CreatePaymentAsync([FromBody] CreatePaymentRequest request)
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
        
        var result = await _paymentService.CreatePaymentAsync(request);
        return Result<object>.Created(result).ToActionResult();
    }

    [HttpGet("internal/confirm/sandbox/{id}")]
    [InternalAuthorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> ConfirmSandboxPayment(string id)
    {
        return (await _paymentService.ConfirmSandboxPaymentAsync(id)).ToActionResult();
    }
}