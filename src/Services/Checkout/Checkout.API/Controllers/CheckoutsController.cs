using Asp.Versioning;
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

    [HttpPost("payment/create")]
    public async Task<IActionResult> CreatePayment()
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("paymentlink/{paymentLinkId}/items")]
    public async Task<IActionResult> GetItemsDetails(string paymentLinkId)
    {
        var result = await _checkoutService.GetPaymentLinkItemsDetailsAsync(paymentLinkId);
        return result.ToActionResult();
    }
}