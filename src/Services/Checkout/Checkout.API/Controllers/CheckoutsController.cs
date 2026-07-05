using System.Diagnostics;
using Asp.Versioning;
using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shared.Kernel.Results;

namespace Checkout.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class CheckoutsController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;
    private readonly IValidator<CreatePaymentRequest> _createPaymentRequestValidator;

    public CheckoutsController(
        ICheckoutService checkoutService, 
        IValidator<CreatePaymentRequest> createPaymentRequestValidator
    )
    {
        _checkoutService = checkoutService;
        _createPaymentRequestValidator = createPaymentRequestValidator;
    }

    [HttpPost("payment/pix")]
    public async Task<IActionResult> CreatePaymentAsync([FromBody] CreatePaymentRequest request)
    {
        var validationResult = await _createPaymentRequestValidator.ValidateAsync(request);

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
        
        var result = await _checkoutService.CreatePixPaymentAsync(request);
        return result.ToActionResult();
    }
    
    [HttpGet("payment/sandbox/confirm/{paymentId}")]
    public async Task<IActionResult> ConfirmSandboxPaymentAsync(string paymentId)
    {
        var result = await _checkoutService.ConfirmSandboxPaymentAsync(paymentId);
        return result.ToActionResult();
    }
    
    [HttpGet("paymentlink/{paymentLinkId}/items")]
    public async Task<IActionResult> GetItemsDetailsAsync(string paymentLinkId)
    {
        var result = await _checkoutService.GetPaymentLinkItemsDetailsAsync(paymentLinkId);
        return result.ToActionResult();
    }
}