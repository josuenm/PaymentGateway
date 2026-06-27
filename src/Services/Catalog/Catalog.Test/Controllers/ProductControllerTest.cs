using Catalog.API.Controllers;
using Catalog.Application.Prices.DTOs.Requests;
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
        var request = new CreateProductRequest("Produto X", "Exemplo", new List<CreatePriceRequest>());

        // _productServiceMock
        //     .Setup(method => method.CreateAsync(
        //         It.IsAny<string>(), 
        //         It.IsAny<CreateProductRequest>()
        //     ))
        //     .ReturnsAsync();
        
        var result = await _productsController.CreateAsync("usr_123", request);
        var resultObject = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<ProductResponse>>(resultObject.Value);
        
        Assert.Equal(201, resultObject.StatusCode);
        
        Assert.Null(resultValue.Error);
        Assert.NotNull(resultValue.Data);
        Assert.True(resultValue.Success);
    }
}