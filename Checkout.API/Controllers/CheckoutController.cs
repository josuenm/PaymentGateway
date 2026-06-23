using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class CheckoutController : ControllerBase
{
    [HttpGet("paymentlink/{paymentLinkId}/items")]
    public Task GetItemsDetails(string paymentLinkId)
    {
        throw new NotImplementedException();
    }
}