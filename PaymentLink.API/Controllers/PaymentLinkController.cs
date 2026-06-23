using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;
using PaymentLink.Application.PaymentLinks.Interfaces;
using Shared.Extensions;

namespace PaymentLink.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class PaymentLinkController : ControllerBase
{
    private readonly IPaymentLinkService _paymentLinkService;

    public PaymentLinkController(IPaymentLinkService paymentLinkService)
    {
        _paymentLinkService = paymentLinkService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreatePaymentLink request, 
        [FromHeader(Name = "X-User-Id")] string userId
    )
    {
        var result = await _paymentLinkService.CreateAsync(request, userId);
        return result.ToActionResult();
    }
}