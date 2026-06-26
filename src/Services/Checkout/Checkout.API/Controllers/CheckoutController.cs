using Asp.Versioning;
using Checkout.Application.Checkouts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }
    
    [HttpGet("paymentlink/{paymentLinkId}/items")]
    public async Task<IActionResult> GetItemsDetails(string paymentLinkId)
    {
        var result = await _checkoutService.GetPaymentLinkItemsDetailsAsync(paymentLinkId);
        return result.ToActionResult();
    }
}