using Catalog.API.Controllers;
using Catalog.Application.Prices.DTOs.Responses;
using Catalog.Application.Prices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Kernel.Results;

namespace Catalog.Test.Controllers;

public class PricesControllerTest
{
    private readonly Mock<IPriceService> _priceServiceMock;
    private readonly PricesController _pricesController;

    public PricesControllerTest()
    {
        _priceServiceMock = new Mock<IPriceService>();
        _pricesController = new PricesController(_priceServiceMock.Object);
    }

    [Fact]
    public async Task InternalGetManyByIdAsync_WithNullIdList_ReturnsBadRequest()
    {
        var result = await _pricesController.InternalGetManyByIdAsync(null);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(objectResult.Value);

        Assert.Equal(400, objectResult.StatusCode);
        Assert.False(resultValue.Success);
        Assert.Equal("A lista de id precisa ser fornecida", resultValue.Error!.Message);
    }

    [Fact]
    public async Task InternalGetManyByIdAsync_WithEmptyIdList_ReturnsBadRequest()
    {
        var result = await _pricesController.InternalGetManyByIdAsync(new List<string>());
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(objectResult.Value);

        Assert.Equal(400, objectResult.StatusCode);
        Assert.False(resultValue.Success);
        Assert.Equal("A lista de id precisa ter pelo menos 1 id", resultValue.Error!.Message);
    }

    [Fact]
    public async Task InternalGetManyByIdAsync_WithValidIdList_ReturnsOk()
    {
        var idList = new List<string> { "pri_123" };

        _priceServiceMock
            .Setup(method => method.InternalGetManyByIdAsync(idList))
            .ReturnsAsync(Result<IEnumerable<InternalPriceResponse>>.Ok(new List<InternalPriceResponse>()));

        var result = await _pricesController.InternalGetManyByIdAsync(idList);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<IEnumerable<InternalPriceResponse>>>(objectResult.Value);

        Assert.Equal(200, objectResult.StatusCode);
        Assert.True(resultValue.Success);
        Assert.NotNull(resultValue.Data);
    }
}
