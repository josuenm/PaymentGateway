using Catalog.API.Controllers;
using Catalog.Application.Prices.DTOs.Requests;
using Catalog.Application.Prices.DTOs.Responses;
using Catalog.Application.Products.DTOs.Requests;
using Catalog.Application.Products.DTOs.Responses;
using Catalog.Application.Products.Interfaces;
using Catalog.Application.Products.Validators;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Kernel.Results;

namespace Catalog.Test.Controllers;

public class ProductControllerTest
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly ProductsController _productsController;

    public ProductControllerTest()
    {
        _productServiceMock = new Mock<IProductService>();
        _productsController = new ProductsController(
            _productServiceMock.Object,
            new CreateProductRequestValidator()
        );
    }

    [Fact]
    public async Task CreateProduct_InvalidData_ReturnsBadRequest()
    {
        var request = new CreateProductRequest("", "", new List<CreatePriceRequest>());
        
        var result = await _productsController.CreateAsync("usr_123", request);
        var resultObject = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(resultObject.Value);
        
        Assert.Equal(400, resultObject.StatusCode);
        
        Assert.Null(resultValue.Data);
        Assert.False(resultValue.Success);
        Assert.NotNull(resultValue.Error);
        Assert.NotNull(resultValue.Error.Details);
        
        Assert.Equal("1 ou mais campos inválidos", resultValue.Error.Message);
        Assert.Contains("name", resultValue.Error.Details.Keys);
    }

    [Fact]
    public async Task CreateProduct_ValidData_ReturnsCreated()
    {
        var request = new CreateProductRequest(
            "Produto X", 
            "Exemplo", 
            new List<CreatePriceRequest>()
        );

        _productServiceMock
            .Setup(method => method.CreateAsync(
                It.IsAny<string>(), 
                It.IsAny<CreateProductRequest>()
            ))
            .ReturnsAsync(Result<ProductResponse>.Created(new ProductResponse(
            "prod_123", 
            request.Name, 
            request.Description, 
            false, 
            true, 
            "usr_123", 
            new List<PriceResponse>(),
            null, 
            DateTime.UtcNow, 
            null
        )));

        var result = await _productsController.CreateAsync("usr_123", request);

        var resultObject = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<ProductResponse>>(resultObject.Value);
        
        Assert.Equal(201, resultObject.StatusCode);
        
        Assert.Null(resultValue.Error);
        Assert.NotNull(resultValue.Data);
        Assert.True(resultValue.Success);
    }
    
    [Fact]
    public async Task GetAllPaged_ReturnsOk()
    {
        var userId = "usr_123";
        var page = 1;
        var limit = 10;

        _productServiceMock
            .Setup(method => method.GetAllPagedAsync(userId, page, limit))
            .ReturnsAsync(new PagedResponse<ProductResponse>(
                new List<ProductResponse>(), 
                0, 
                page, 
                1, 
                limit
            ));
        
        var result = await _productsController.GetAllPagedAsync(userId, page, limit);
        var resultObject = Assert.IsType<OkObjectResult>(result);
        var resultValue = Assert.IsType<PagedResponse<ProductResponse>>(resultObject.Value);
        
        Assert.Equal(200, resultObject.StatusCode);
        
        Assert.Equal(page, resultValue.Page);
        Assert.Equal(limit, resultValue.Limit);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOk()
    {
        var userId = "usr_123";
        var productId = "prod_123";

        var productResponse = new ProductResponse(
            productId, 
            "Produto X", 
            null, 
            false, 
            true, 
            userId, 
            new List<PriceResponse>(),
            null, 
            DateTime.UtcNow, 
            null
        );

        _productServiceMock
            .Setup(method => method.GetByIdAsync(userId, productId))
            .ReturnsAsync(Result<ProductResponse>.Ok(productResponse));
        
        var result = await _productsController.GetByIdAsync(userId, productId);
        var resultObject = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<ProductResponse>>(resultObject.Value);
        
        Assert.Equal(200, resultObject.StatusCode);
        
        Assert.Null(resultValue.Error);
        Assert.NotNull(resultValue.Data);
        Assert.True(resultValue.Success);
        
        Assert.Equal(productResponse.Id, resultValue.Data.Id);
        Assert.Equal(productResponse.Name, resultValue.Data.Name);
        Assert.Equal(productResponse.Description, resultValue.Data.Description);
        Assert.Equal(productResponse.IsActive, resultValue.Data.IsActive);
        Assert.Equal(productResponse.UserId, resultValue.Data.UserId);
        Assert.Equal(productResponse.CreatedAt, resultValue.Data.CreatedAt);
        Assert.Equal(productResponse.UpdatedAt, resultValue.Data.UpdatedAt);
    }
}