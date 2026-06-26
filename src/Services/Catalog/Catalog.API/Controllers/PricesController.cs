using Asp.Versioning;
using Catalog.Application.Prices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Attributes;
using Shared.Kernel.Results;

namespace Catalog.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PricesController : ControllerBase
{
    private readonly IPriceService _priceService;
    
    public PricesController(IPriceService priceService)
    {
        _priceService = priceService;
    }
    
    [HttpGet("internal")]
    [InternalAuthorize]
    public async Task<IActionResult> InternalGetManyByIdAsync([FromQuery] IEnumerable<string>? idList)
    {
        if (idList == null)
            return Result<object>.BadRequest("A lista de id precisa ser fornecida").ToActionResult();
        
        if (!idList.Any())
            return Result<object>.BadRequest("A lista de id precisa ter pelo menos 1 id").ToActionResult();
        
        var result = await _priceService.InternalGetManyByIdAsync(idList);
        return result.ToActionResult();
    }
}