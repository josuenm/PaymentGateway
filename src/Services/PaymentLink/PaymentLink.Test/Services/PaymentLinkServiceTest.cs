using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;
using PaymentLink.Application.PaymentLinks.DTOs.Responses;
using PaymentLink.Application.PaymentLinks.Services;
using PaymentLink.Domain.Entities;
using PaymentLink.Domain.PaymentLinks.Enums;
using PaymentLink.Domain.PaymentLinks.Repositories;
using Shared.Kernel.Results;

namespace PaymentLink.Test.Services;

public class PaymentLinkServiceTest
{
    private readonly Mock<IPaymentLinkRepository> _paymentLinkRepository;
    private readonly Mock<IHttpClientFactory> _httpClientFactory;

    public PaymentLinkServiceTest()
    {
        _paymentLinkRepository = new Mock<IPaymentLinkRepository>();
        _httpClientFactory = new Mock<IHttpClientFactory>();
    }

    private PaymentLinkService CreateService(HttpResponseMessage httpResponse)
    {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponse))
        {
            BaseAddress = new Uri("http://localhost")
        };

        _httpClientFactory
            .Setup(factory => factory.CreateClient("CatalogClient"))
            .Returns(httpClient);

        return new PaymentLinkService(_httpClientFactory.Object, _paymentLinkRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidPrices_ReturnsBadRequest()
    {
        var userId = "usr_123";
        var request = new CreatePaymentLink(
            new List<PaymentLinkMethods> { PaymentLinkMethods.Pix },
            new List<CreatePaymentLinkItem> { new("pri_123", 1), new("pri_456", 1) }
        );

        var pricesResult = new ResultObject<IEnumerable<PriceResponse>>(
            new List<PriceResponse>
            {
                new("pri_123", "Produto X", 1000, "BRL", "prod_123", true, "OneTime", null, userId)
            },
            true,
            null
        );

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(pricesResult), Encoding.UTF8, "application/json")
        };

        var service = CreateService(httpResponse);
        var result = await service.CreateAsync(request, userId);
        var resultObject = Assert.IsType<Result<PaymentLinkResponse>>(result);

        Assert.Equal(400, resultObject.StatusCode);
        Assert.False(resultObject.Success);
        Assert.Equal("Algum preço é inválido", resultObject.Error!.Message);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreated()
    {
        var userId = "usr_123";
        var priceId = "pri_123";
        var request = new CreatePaymentLink(
            new List<PaymentLinkMethods> { PaymentLinkMethods.Pix },
            new List<CreatePaymentLinkItem> { new(priceId, 1) }
        );

        var pricesResult = new ResultObject<IEnumerable<PriceResponse>>(
            new List<PriceResponse>
            {
                new(priceId, "Produto X", 1000, "BRL", "prod_123", true, "OneTime", null, userId)
            },
            true,
            null
        );

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(pricesResult), Encoding.UTF8, "application/json")
        };

        _paymentLinkRepository
            .Setup(method => method.CreateAsync(It.IsAny<PaymentLinkEntity>()))
            .ReturnsAsync((PaymentLinkEntity paymentLink) => paymentLink);

        var service = CreateService(httpResponse);
        var result = await service.CreateAsync(request, userId);
        var resultObject = Assert.IsType<Result<PaymentLinkResponse>>(result);

        Assert.Equal(201, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
    }

    [Fact]
    public async Task InternalGetByIdAsync_WithInvalidId_ReturnsNotFound()
    {
        var paymentLinkId = "plink_123";

        _paymentLinkRepository
            .Setup(method => method.GetByIdAsync(paymentLinkId, true))
            .ReturnsAsync((PaymentLinkEntity?)null);

        var service = new PaymentLinkService(_httpClientFactory.Object, _paymentLinkRepository.Object);
        var result = await service.InternalGetByIdAsync(paymentLinkId);
        var resultObject = Assert.IsType<Result<InternalPaymentLinkResponse>>(result);

        Assert.Equal(404, resultObject.StatusCode);
        Assert.False(resultObject.Success);
        Assert.Equal("Link de pagamento não encontrado", resultObject.Error!.Message);
    }

    [Fact]
    public async Task InternalGetByIdAsync_WithValidId_ReturnsOk()
    {
        var userId = "usr_123";
        var paymentLink = new PaymentLinkEntity(userId, new List<PaymentLinkMethods> { PaymentLinkMethods.Pix }, true, true);
        paymentLink.SetItems(new List<PaymentLinkItem>
        {
            new("pri_123", userId, paymentLink.Id, 1, true)
        });

        _paymentLinkRepository
            .Setup(method => method.GetByIdAsync(paymentLink.Id, true))
            .ReturnsAsync(paymentLink);

        var service = new PaymentLinkService(_httpClientFactory.Object, _paymentLinkRepository.Object);
        var result = await service.InternalGetByIdAsync(paymentLink.Id);
        var resultObject = Assert.IsType<Result<InternalPaymentLinkResponse>>(result);

        Assert.Equal(200, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
        Assert.Equal(userId, resultObject.Data.UserId);
        Assert.Single(resultObject.Data.Items);
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            return Task.FromResult(_response);
        }
    }
}
