using Asp.Versioning;
using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class CheckoutsController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutsController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost("payment/pix")]
    public async Task<IActionResult> CreatePaymentAsync([FromBody] CreatePaymentRequest request)
    {
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