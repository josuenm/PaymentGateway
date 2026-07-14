using MassTransit;
using Moq;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;
using PaymentLink.Application.PaymentLinks.DTOs.Responses;
using PaymentLink.Application.PaymentLinks.Interfaces;
using PaymentLink.Application.PaymentLinks.Messaging.Commands;
using PaymentLink.Application.PaymentLinks.Services;
using PaymentLink.Application.ProductReadModels.Interfaces;
using PaymentLink.Domain.PaymentLinks.Entities;
using PaymentLink.Domain.PaymentLinks.Enums;
using PaymentLink.Domain.PaymentLinks.Repositories;
using PaymentLink.Domain.ProductReadModels.Entities;
using PaymentLink.Domain.ProductReadModels.Enums;
using Shared.Kernel.Results;

namespace PaymentLink.Test.Services;

public class PaymentLinkServiceTest
{
    private readonly IPaymentLinkService _paymentLinkService;
    private readonly Mock<IPublishEndpoint> _publishEndpoint;
    private readonly Mock<IPriceReadModelService> _priceReadModelService;
    private readonly Mock<IPaymentLinkRepository> _paymentLinkRepository;

    public PaymentLinkServiceTest()
    {
        _publishEndpoint = new Mock<IPublishEndpoint>();
        _priceReadModelService = new Mock<IPriceReadModelService>();
        _paymentLinkRepository = new Mock<IPaymentLinkRepository>();

        _paymentLinkService = new PaymentLinkService(
            _priceReadModelService.Object, 
            _paymentLinkRepository.Object, 
            _publishEndpoint.Object
        );
    }

    [Fact]
    public async Task CreateAsync_WithInvalidPrices_ReturnsBadRequest()
    {
        var userId = "usr_123";
        var request = new CreatePaymentLink(
            new List<PaymentLinkMethods> { PaymentLinkMethods.Pix },
            new List<CreatePaymentLinkItem> { new("pri_123", 1), new("pri_456", 1) }
        );

        _priceReadModelService
            .Setup(m => m.GetManyByIdAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<PriceReadModel>
            {
                new(
                    "pri_123", 
                    "Produto X", 
                    PriceReadModelFrequency.OneTime, 
                    PriceReadModelCycle.Monthly , 
                    "prod_123",
                    10000,
                    "BRL", 
                    "usr_123",
                    true
                )
            });

        var result = await _paymentLinkService.CreateAsync(request, userId);
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

        _priceReadModelService
            .Setup(m => m.GetManyByIdAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<PriceReadModel>
            {
                new(
                    priceId, 
                    "Produto X", 
                    PriceReadModelFrequency.OneTime, 
                    PriceReadModelCycle.Monthly , 
                    "prod_123",
                    10000,
                    "BRL", 
                    "usr_123",
                    true
                )
            });

        _paymentLinkRepository
            .Setup(m => m.CreateAsync(It.IsAny<PaymentLinkEntity>()))
            .ReturnsAsync((PaymentLinkEntity paymentLink) => paymentLink);

        var result = await _paymentLinkService.CreateAsync(request, userId);

        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Null(result.Error);
        Assert.True(result.Success);
        Assert.Single(result.Data.Items);

        _publishEndpoint.Verify(
            p => p.Publish(
                It.Is<PaymentLinkCreatedCommand>(cmd => cmd.UserId == userId && cmd.IsActive),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}
