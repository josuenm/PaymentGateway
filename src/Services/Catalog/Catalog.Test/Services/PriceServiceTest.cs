using Catalog.Application.Prices.DTOs.Responses;
using Catalog.Application.Prices.Services;
using Catalog.Domain.Prices.Entities;
using Catalog.Domain.Prices.Enums;
using Catalog.Domain.Prices.Repositories;
using Moq;
using Shared.Kernel.Results;

namespace Catalog.Test.Services;

public class PriceServiceTest
{
    private readonly PriceService _priceService;
    private readonly Mock<IPriceRepository> _priceRepository;

    public PriceServiceTest()
    {
        _priceRepository = new Mock<IPriceRepository>();
        _priceService = new PriceService(_priceRepository.Object);
    }

    [Fact]
    public async Task InternalGetManyByIdAsync_WithValidIds_ReturnsOk()
    {
        var userId = "usr_123";
        var productId = "prod_123";
        var idList = new List<string> { "pri_123" };
        var price = new Price("Preço X", 1000, "BRL", PriceFrequency.OneTime, null, productId, userId);

        _priceRepository
            .Setup(method => method.GetManyByIdAsync(idList, true, true))
            .ReturnsAsync(new List<Price> { price });

        var result = await _priceService.InternalGetManyByIdAsync(idList);
        var resultObject = Assert.IsType<Result<IEnumerable<InternalPriceResponse>>>(result);

        Assert.Equal(200, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);

        var priceResponse = Assert.Single(resultObject.Data);
        Assert.Equal(price.Id, priceResponse.Id);
        Assert.Equal(price.Name, priceResponse.Name);
        Assert.Equal(price.Amount, priceResponse.Amount);
    }
}
