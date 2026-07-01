using Catalog.Application.Prices.DTOs.Requests;
using Catalog.Application.Products.DTOs.Requests;
using Catalog.Application.Products.DTOs.Responses;
using Catalog.Application.Products.Services;
using Catalog.Domain.Commons;
using Catalog.Domain.Prices.Enums;
using Catalog.Domain.Products.Entities;
using Catalog.Domain.Products.Repositories;
using Moq;
using Shared.Kernel.Results;

namespace Catalog.Test.Services;

public class ProductServiceTest
{
    private readonly ProductService _productService;
    private readonly Mock<IProductRepository> _productRepository;

    public ProductServiceTest()
    {
        _productRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_productRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreated()
    {
        var userId = "usr_123";
        var request = new CreateProductRequest("Produto X", "Exemplo", new List<CreatePriceRequest>());

        _productRepository
            .Setup(method => method.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product product) => product);

        var result = await _productService.CreateAsync(userId, request);
        var resultObject = Assert.IsType<Result<ProductResponse>>(result);

        Assert.Equal(201, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
        Assert.Equal(request.Name, resultObject.Data.Name);
        Assert.Equal(userId, resultObject.Data.UserId);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNotFound()
    {
        var userId = "usr_123";
        var productId = "prod_123";

        _productRepository
            .Setup(method => method.GetByIdAsync(userId, productId))
            .ReturnsAsync((Product?)null);

        var result = await _productService.GetByIdAsync(userId, productId);
        var resultObject = Assert.IsType<Result<ProductResponse>>(result);

        Assert.Equal(404, resultObject.StatusCode);
        Assert.Null(resultObject.Data);
        Assert.NotNull(resultObject.Error);
        Assert.False(resultObject.Success);
        Assert.Equal("O produto não foi encontrado", resultObject.Error.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOk()
    {
        var userId = "usr_123";
        var product = new Product("Produto X", userId, true, true, "Exemplo");

        _productRepository
            .Setup(method => method.GetByIdAsync(userId, product.Id))
            .ReturnsAsync(product);

        var result = await _productService.GetByIdAsync(userId, product.Id);
        var resultObject = Assert.IsType<Result<ProductResponse>>(result);

        Assert.Equal(200, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
        Assert.Equal(product.Id, resultObject.Data.Id);
        Assert.Equal(product.Name, resultObject.Data.Name);
    }

    [Fact]
    public async Task GetAllPagedAsync_ReturnsPagedResponse()
    {
        var userId = "usr_123";
        var page = 1;
        var limit = 10;
        var product = new Product("Produto X", userId, true, true);

        _productRepository
            .Setup(method => method.GetAllPagedAsync(userId, page, limit))
            .ReturnsAsync(new PagedSearchResult<Product>(new List<Product> { product }, 1));

        var result = await _productService.GetAllPagedAsync(userId, page, limit);

        Assert.Equal(page, result.Page);
        Assert.Equal(limit, result.Limit);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }
}
