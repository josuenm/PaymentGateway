using Asp.Versioning;
using Catalog.Application.Products.DTOs.Requests;
using Catalog.Application.Products.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Kernel.Results;

namespace Catalog.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    
    private readonly IValidator<CreateProductRequest> _createProductRequestValidator;
    
    public ProductsController(
        IProductService productService, 
        IValidator<CreateProductRequest> createProductRequestValidator
    )
    {
        _productService = productService;
        _createProductRequestValidator = createProductRequestValidator;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromHeader(Name = "X-User-Id")] string userId, 
        [FromBody] CreateProductRequest? request
    )
    {
        if (request is null)
        {
            return Result<object>
                .Failure(
                    "O corpo da requisição (JSON) está malformado ou contém tipos de dados inválidos (como Enums incorretos).", 
                    ErrorType.Validation)
                .ToActionResult();
        }
        
        var validationResult = await _createProductRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Result<object>
                .Failure(
                    "1 ou mais campos inválidos", 
                    ErrorType.Validation, 
                    validationResult.Errors)
                .ToActionResult();

        var result = await _productService.CreateAsync(userId, request);
        return result.ToActionResult();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPagedAsync(
        [FromHeader(Name = "X-User-Id")] string userId,
        [FromQuery] int page = 1, 
        [FromQuery] int limit = 10
    )
    {
        return Ok(await _productService.GetAllPagedAsync(userId, page, limit));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByIdAsync([FromHeader(Name = "X-User-Id")] string userId, string id)
    {
        var result = await _productService.GetByIdAsync(userId, id);
        return result.ToActionResult();
    }
}