using System.Diagnostics;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;
using PaymentLink.Application.PaymentLinks.DTOs.Responses;
using PaymentLink.Application.PaymentLinks.Interfaces;
using Shared.DTOs.Responses;
using Shared.Infrastructure.Attributes;
using Shared.Kernel.Results;

namespace PaymentLink.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class PaymentLinksController : ControllerBase
{
    private readonly IPaymentLinkService _paymentLinkService;
    
    private readonly IValidator<CreatePaymentLink> _createPaymentLinkValidator;

    public PaymentLinksController(
        IPaymentLinkService paymentLinkService, 
        IValidator<CreatePaymentLink> createPaymentLinkValidator
    )
    {
        _paymentLinkService = paymentLinkService;
        _createPaymentLinkValidator = createPaymentLinkValidator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResultObject<PaymentLinkResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultObject<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreatePaymentLink request, 
        [FromHeader(Name = "X-User-Id")] string userId
    )
    {
        var validationResult = await _createPaymentLinkValidator.ValidateAsync(request);

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
        
        var result = await _paymentLinkService.CreateAsync(request, userId);
        return result.ToActionResult();
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(ResultObject<PagedResponse<PaymentLinkResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPagedAsync(
        [FromHeader(Name = "X-User-Id")] string userId, 
        [FromQuery] int page = 1, 
        [FromQuery] int limit = 10
    )
    {
        var result = await _paymentLinkService.GetAllPagedAsync(userId, page, limit);
        return result.ToActionResult();
    }
}