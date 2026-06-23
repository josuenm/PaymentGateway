using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;
using PaymentLink.Application.PaymentLinks.Interfaces;
using Shared.Extensions;
using Shared.Kernel.Results;

namespace PaymentLink.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class PaymentLinkController : ControllerBase
{
    private readonly IPaymentLinkService _paymentLinkService;
    
    private readonly IValidator<CreatePaymentLink> _createPaymentLinkValidator;

    public PaymentLinkController(
        IPaymentLinkService paymentLinkService, 
        IValidator<CreatePaymentLink> createPaymentLinkValidator
    )
    {
        _paymentLinkService = paymentLinkService;
        _createPaymentLinkValidator = createPaymentLinkValidator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreatePaymentLink request, 
        [FromHeader(Name = "X-User-Id")] string userId
    )
    {
        var validationResult = await _createPaymentLinkValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Result<object>
                .Failure(
                    "1 ou mais campos inválidos", 
                    ErrorType.Validation, 
                    validationResult.Errors
                )
                .ToActionResult();
        }
        
        var result = await _paymentLinkService.CreateAsync(request, userId);
        return result.ToActionResult();
    }

    [HttpGet("/internal/{id}")]
    public async Task<IActionResult> GetInternalPaymentLinkAsync(string id)
    {
        var result = await _paymentLinkService.GetInternalByIdAsync(id);
        return result.ToActionResult();
    }
}