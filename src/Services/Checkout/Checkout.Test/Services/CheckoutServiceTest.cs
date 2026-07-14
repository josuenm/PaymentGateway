using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.DTOs.Responses;
using Checkout.Application.Checkouts.Services;
using Checkout.Application.CustomerReadModels.Interfaces;
using Checkout.Application.CustomerReadModels.Messaging.Commands;
using Checkout.Application.PaymentLinkReadModels.Interfaces;
using Checkout.Application.ProductReadModels.Interfaces;
using Checkout.Domain.Checkouts.Entities;
using Checkout.Domain.Checkouts.Enums;
using Checkout.Domain.Checkouts.Repositories;
using Checkout.Domain.CustomerReadModels.Entities;
using Checkout.Domain.PaymentLinkReadModels.Entities;
using Checkout.Domain.ProductReadModels.Entities;
using Checkout.Domain.ProductReadModels.Enums;
using MassTransit;
using Moq;

namespace Checkout.Test.Services;

public class CheckoutServiceTest
{
    private readonly CheckoutService _checkoutService;
    private readonly Mock<ICheckoutSessionRepository> _checkoutSessionRepository;
    private readonly Mock<ICustomerReadModelService> _customerReadModelService;
    private readonly Mock<IPaymentLinkReadModelService> _paymentLinkReadModelService;
    private readonly Mock<IPriceReadModelService> _priceReadModelService;
    private readonly Mock<IPublishEndpoint> _publishEndpoint;
    private readonly Mock<IPaymentApiClient> _paymentApiClient;

    public CheckoutServiceTest()
    {
        _checkoutSessionRepository = new Mock<ICheckoutSessionRepository>();
        _customerReadModelService = new Mock<ICustomerReadModelService>();
        _paymentLinkReadModelService = new Mock<IPaymentLinkReadModelService>();
        _priceReadModelService = new Mock<IPriceReadModelService>();
        _publishEndpoint = new Mock<IPublishEndpoint>();
        _paymentApiClient = new Mock<IPaymentApiClient>();

        _checkoutService = new CheckoutService(
            _checkoutSessionRepository.Object,
            _customerReadModelService.Object,
            _paymentLinkReadModelService.Object,
            _priceReadModelService.Object,
            _publishEndpoint.Object,
            _paymentApiClient.Object
        );
    }

    [Fact]
    public async Task CreatePaymentAsync_WithNullPaymentLink_ReturnsInternalServerError()
    {
        var request = new CreatePaymentRequest(
            PaymentMethod.Pix,
            "plink_123",
            new CustomerPaymentRequest("John Doe", "example@example.com", "12345678901", null)
        );

        _paymentLinkReadModelService
            .Setup(m => m.GetByIdAsync(request.SourceId))
            .ReturnsAsync((PaymentLinkReadModel?)null);

        var result = await _checkoutService.CreatePaymentAsync(request);

        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Success);
        Assert.Equal("Erro ao obter o link de pagamento", result.Error!.Message);
    }

    [Fact]
    public async Task CreatePaymentAsync_WithInactivePaymentLink_ReturnsInternalServerError()
    {
        var request = new CreatePaymentRequest(
            PaymentMethod.Pix,
            "plink_123",
            new CustomerPaymentRequest("John Doe", "example@example.com", "12345678901", null)
        );

        var inactivePaymentLink = new PaymentLinkReadModel("plink_123", isActive: false, userId: "usr_123", liveMode: true);

        _paymentLinkReadModelService
            .Setup(m => m.GetByIdAsync(request.SourceId))
            .ReturnsAsync(inactivePaymentLink);

        var result = await _checkoutService.CreatePaymentAsync(request);

        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Success);
        Assert.Equal("O link de pagamento não esta ativo", result.Error!.Message);
    }

    [Fact]
    public async Task CreatePaymentAsync_WithNewCustomer_PublishesCreateCustomerCommandAndReturnsCreated()
    {
        var userId = "usr_123";
        var priceId = "pri_123";
        var request = new CreatePaymentRequest(
            PaymentMethod.Pix,
            "plink_123",
            new CustomerPaymentRequest("John Doe", "example@example.com", "12345678901", null)
        );

        var paymentLink = new PaymentLinkReadModel("plink_123", isActive: true, userId: userId, liveMode: true);
        paymentLink.SetItems(new List<PaymentLinkItemReadModel>
        {
            new("pli_123", "plink_123", "pri_123", 1, true)
        });

        _paymentLinkReadModelService
            .Setup(m => m.GetByIdAsync(request.SourceId))
            .ReturnsAsync(paymentLink);

        _customerReadModelService
            .Setup(m => m.GetByEmailAndUserIdAsync(userId, request.Customer.Email))
            .ReturnsAsync((CustomerReadModel?)null);

        _priceReadModelService
            .Setup(m => m.GetManyByIdAsync(It.Is<IEnumerable<string>>(ids => ids.Contains(priceId))))
            .ReturnsAsync(new List<PriceReadModel>
            {
                new(
                    priceId, 
                    "Produto X", 
                    PriceReadModelFrequency.OneTime, 
                    null, 
                    "prod_123", 
                    1000, 
                    "BRL", 
                    userId, 
                    true
                )
            });

        _paymentApiClient
            .Setup(m => m.CreatePaymentAsync(It.IsAny<CreatePaymentHttpRequest>()))
            .ReturnsAsync(new PaymentHttpResponse(
                "QR_CODE_DATA", 
                "payt_123", 
                10000, 
                "BRL", 
                "PENDING"
            ));

        _checkoutSessionRepository
            .Setup(m => m.CreateAsync(It.IsAny<CheckoutSession>()))
            .ReturnsAsync((CheckoutSession session) => session);

        var result = await _checkoutService.CreatePaymentAsync(request);

        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Null(result.Error);
        Assert.True(result.Success);
        Assert.Equal("payt_123", result.Data.PaymentId);
        Assert.Equal("QR_CODE_DATA", result.Data.QrCodeData);
        Assert.Equal(1000, result.Data.Amount);

        _customerReadModelService.Verify(m => m.CreateAsync(It.IsAny<CustomerReadModel>()), Times.Once);

        _publishEndpoint.Verify(
            p => p.Publish(
                It.Is<CreateCustomerCommand>(cmd => cmd.Email == request.Customer.Email),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _publishEndpoint.Verify(
            p => p.Publish(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task CreatePaymentAsync_WithExistingCustomerMissingTaxId_PublishesUpdateCustomerCommand()
    {
        var userId = "usr_123";
        var priceId = "pri_123";
        var request = new CreatePaymentRequest(
            PaymentMethod.Pix,
            "plink_123",
            new CustomerPaymentRequest("John Doe", "example@example.com", "12345678901", null)
        );

        var paymentLink = new PaymentLinkReadModel("plink_123", isActive: true, userId: userId, liveMode: true);
        paymentLink.SetItems(new List<PaymentLinkItemReadModel>
        {
            new("pli_123", "plink_123", priceId, 1, true)
        });

        _paymentLinkReadModelService
            .Setup(m => m.GetByIdAsync(request.SourceId))
            .ReturnsAsync(paymentLink);

        var existingCustomer = new CustomerReadModel("cust_123", request.Customer.Email, null, null, userId, false);

        _customerReadModelService
            .Setup(m => m.GetByEmailAndUserIdAsync(userId, request.Customer.Email))
            .ReturnsAsync(existingCustomer);

        _priceReadModelService
            .Setup(m => m.GetManyByIdAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<PriceReadModel>
            {
                new(priceId, "Produto X", PriceReadModelFrequency.OneTime, null, "prod_123", 1000, "BRL", userId, true)
            });

        _paymentApiClient
            .Setup(m => m.CreatePaymentAsync(It.IsAny<CreatePaymentHttpRequest>()))
            .ReturnsAsync(new PaymentHttpResponse(
                "QR_CODE_DATA", 
                "payt_123", 
                10000, 
                "BRL", 
                "PENDING"
            ));

        _checkoutSessionRepository
            .Setup(m => m.CreateAsync(It.IsAny<CheckoutSession>()))
            .ReturnsAsync((CheckoutSession session) => session);

        var result = await _checkoutService.CreatePaymentAsync(request);

        Assert.Equal(201, result.StatusCode);

        _customerReadModelService.Verify(m => m.CreateAsync(It.IsAny<CustomerReadModel>()), Times.Never);
        _publishEndpoint.Verify(p => p.Publish(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        _customerReadModelService.Verify(m => m.UpdateAsync(It.IsAny<CustomerReadModel>()), Times.Once);
        _publishEndpoint.Verify(
            p => p.Publish(
                It.Is<UpdateCustomerCommand>(cmd => cmd.TaxId == request.Customer.TaxId),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenPaymentGatewayFails_ReturnsInternalServerError()
    {
        var userId = "usr_123";
        var priceId = "pri_123";
        var request = new CreatePaymentRequest(
            PaymentMethod.Pix,
            "plink_123",
            new CustomerPaymentRequest("John Doe", "example@example.com", "12345678901", null)
        );

        var paymentLink = new PaymentLinkReadModel("plink_123", isActive: true, userId: userId, liveMode: true);
        paymentLink.SetItems(new List<PaymentLinkItemReadModel>
        {
            new("pli_123",  "plink_123", priceId, 1, true)
        });

        _paymentLinkReadModelService
            .Setup(m => m.GetByIdAsync(request.SourceId))
            .ReturnsAsync(paymentLink);

        _customerReadModelService
            .Setup(m => m.GetByEmailAndUserIdAsync(userId, request.Customer.Email))
            .ReturnsAsync(new CustomerReadModel("cust_123", request.Customer.Email, "John Doe", "12345678901", userId, false));

        _priceReadModelService
            .Setup(m => m.GetManyByIdAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<PriceReadModel>
            {
                new(priceId, "Produto X", PriceReadModelFrequency.OneTime, null, "prod_123", 1000, "BRL", userId, true)
            });

        _paymentApiClient
            .Setup(m => m.CreatePaymentAsync(It.IsAny<CreatePaymentHttpRequest>()))
            .ReturnsAsync((PaymentHttpResponse?)null);

        var result = await _checkoutService.CreatePaymentAsync(request);

        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Success);
        Assert.Equal("Erro ao gerar cobrança", result.Error!.Message);

        _checkoutSessionRepository.Verify(m => m.CreateAsync(It.IsAny<CheckoutSession>()), Times.Never);
    }

    [Fact]
    public async Task GetPaymentLinkItemsDetailsAsync_WithNullPaymentLink_ReturnsInternalServerError()
    {
        var paymentLinkId = "plink_123";

        _paymentLinkReadModelService
            .Setup(m => m.GetByIdAsync(paymentLinkId))
            .ReturnsAsync((PaymentLinkReadModel?)null);

        var result = await _checkoutService.GetPaymentLinkItemsDetailsAsync(paymentLinkId);

        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Success);
        Assert.Equal("Erro ao obter o link de pagamento", result.Error!.Message);
    }

    [Fact]
    public async Task GetPaymentLinkItemsDetailsAsync_WithInactivePaymentLink_ReturnsOkWithEmptyItems()
    {
        var paymentLinkId = "plink_123";
        var inactivePaymentLink = new PaymentLinkReadModel(paymentLinkId, isActive: false, userId: "usr_123", liveMode: true);

        _paymentLinkReadModelService
            .Setup(m => m.GetByIdAsync(paymentLinkId))
            .ReturnsAsync(inactivePaymentLink);

        var result = await _checkoutService.GetPaymentLinkItemsDetailsAsync(paymentLinkId);

        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.False(result.Data.IsActive);
        Assert.Empty(result.Data.items);
    }

    [Fact]
    public async Task ConfirmSandboxPaymentAsync_WithNullPaymentLink_ReturnsInternalServerError()
    {
        var paymentLinkId = "plink_123";

        _paymentLinkReadModelService
            .Setup(m => m.GetByIdAsync(paymentLinkId))
            .ReturnsAsync((PaymentLinkReadModel?)null);

        var result = await _checkoutService.ConfirmSandboxPaymentAsync(paymentLinkId);

        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Success);
        Assert.Equal("Erro ao obter o link de pagamento", result.Error!.Message);
    }

    [Fact]
    public async Task ConfirmSandboxPaymentAsync_WithNonSandboxPaymentLink_ReturnsInternalServerError()
    {
        var paymentLinkId = "plink_123";
        var nonSandboxLink = new PaymentLinkReadModel(paymentLinkId, isActive: true, userId: "usr_123", liveMode: false);

        _paymentLinkReadModelService
            .Setup(m => m.GetByIdAsync(paymentLinkId))
            .ReturnsAsync(nonSandboxLink);

        var result = await _checkoutService.ConfirmSandboxPaymentAsync(paymentLinkId);

        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Success);
        Assert.Equal("O link de pagamento não é sandbox", result.Error!.Message);
    }

    [Fact]
    public async Task ConfirmSandboxPaymentAsync_WithValidSandboxPaymentLink_ReturnsOk()
    {
        var paymentLinkId = "plink_123";
        var sandboxLink = new PaymentLinkReadModel(paymentLinkId, isActive: true, userId: "usr_123", liveMode: true);

        _paymentLinkReadModelService
            .Setup(m => m.GetByIdAsync(paymentLinkId))
            .ReturnsAsync(sandboxLink);

        _paymentApiClient
            .Setup(m => m.ConfirmSandboxPaymentAsync(paymentLinkId))
            .ReturnsAsync(true);

        var result = await _checkoutService.ConfirmSandboxPaymentAsync(paymentLinkId);

        Assert.Equal(200, result.StatusCode);
        Assert.True(result.Success);
        Assert.True(result.Data);
    }
}