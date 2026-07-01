using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.DTOs.Responses;
using Checkout.Application.Checkouts.Services;
using Checkout.Domain.Checkouts.Entities;
using Checkout.Domain.Checkouts.Enums;
using Checkout.Domain.Checkouts.Repositories;
using Checkout.Infrastructure.Http.Interfaces;
using Moq;
using Shared.Kernel.Results;

namespace Checkout.Test.Services;

public class CheckoutServiceTest
{
    private readonly CheckoutService _checkoutService;
    private readonly Mock<ICheckoutSessionRepository> _checkoutSessionRepository;
    private readonly Mock<ICustomerApiClient> _customerApiClient;
    private readonly Mock<IPaymentLinkApiClient> _paymentLinkApiClient;
    private readonly Mock<IPaymentApiClient> _paymentApiClient;
    private readonly Mock<IPriceApiClient> _priceApiClient;

    public CheckoutServiceTest()
    {
        _checkoutSessionRepository = new Mock<ICheckoutSessionRepository>();
        _customerApiClient = new Mock<ICustomerApiClient>();
        _paymentLinkApiClient = new Mock<IPaymentLinkApiClient>();
        _paymentApiClient = new Mock<IPaymentApiClient>();
        _priceApiClient = new Mock<IPriceApiClient>();

        _checkoutService = new CheckoutService(
            _checkoutSessionRepository.Object,
            _customerApiClient.Object,
            _paymentLinkApiClient.Object,
            _paymentApiClient.Object,
            _priceApiClient.Object
        );
    }

    [Fact]
    public async Task CreatePixPaymentAsync_WithNullPaymentLink_ReturnsInternalServerError()
    {
        var request = new CreatePaymentRequest(
            PaymentMethod.Pix,
            "plink_123",
            new CustomerPaymentRequest("John Doe", "example@example.com", "12345678901", null)
        );

        _paymentLinkApiClient
            .Setup(method => method.GetAsync(request.SourceId))
            .ReturnsAsync((PaymentLinkHttpResponse?)null);

        var result = await _checkoutService.CreatePixPaymentAsync(request);
        var resultObject = Assert.IsType<Result<PixPaymentResponse>>(result);

        Assert.Equal(500, resultObject.StatusCode);
        Assert.False(resultObject.Success);
        Assert.Equal("Erro ao obter o link de pagamento", resultObject.Error!.Message);
    }

    [Fact]
    public async Task CreatePixPaymentAsync_WithInactivePaymentLink_ReturnsInternalServerError()
    {
        var request = new CreatePaymentRequest(
            PaymentMethod.Pix,
            "plink_123",
            new CustomerPaymentRequest("John Doe", "example@example.com", "12345678901", null)
        );

        _paymentLinkApiClient
            .Setup(method => method.GetAsync(request.SourceId))
            .ReturnsAsync(new PaymentLinkHttpResponse(
                new List<PaymentLinkItemHttpResponse>(),
                false,
                true,
                "usr_123"
            ));

        var result = await _checkoutService.CreatePixPaymentAsync(request);
        var resultObject = Assert.IsType<Result<PixPaymentResponse>>(result);

        Assert.Equal(500, resultObject.StatusCode);
        Assert.False(resultObject.Success);
        Assert.Equal("O link de pagamento não esta ativo", resultObject.Error!.Message);
    }

    [Fact]
    public async Task CreatePixPaymentAsync_WithValidData_ReturnsCreated()
    {
        var userId = "usr_123";
        var priceId = "pri_123";
        var request = new CreatePaymentRequest(
            PaymentMethod.Pix,
            "plink_123",
            new CustomerPaymentRequest("John Doe", "example@example.com", "12345678901", null)
        );

        _paymentLinkApiClient
            .Setup(method => method.GetAsync(request.SourceId))
            .ReturnsAsync(new PaymentLinkHttpResponse(
                new List<PaymentLinkItemHttpResponse> { new(priceId, 1) },
                true,
                true,
                userId
            ));

        _customerApiClient
            .Setup(method => method.GetOrCreateAsync(userId, It.IsAny<CreateCustomerHttpRequest>()))
            .ReturnsAsync(new CustomerHttpResponse(
                "cust_123",
                request.Customer.Email,
                request.Customer.Name,
                request.Customer.TaxId,
                DateTime.UtcNow
            ));

        _priceApiClient
            .Setup(method => method.GetManyByIdAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<PriceHttpResponse>
            {
                new(priceId, "Produto X", 1000, "BRL", "prod_123", true, "OneTime", null)
            });

        _paymentApiClient
            .Setup(method => method.CreatePixPaymentAsync(It.IsAny<CreatePixPaymentHttpRequest>()))
            .ReturnsAsync(new PixPaymentHttpResponse(
                "QR_CODE_DATA",
                "payt_123",
                1000,
                "BRL",
                "Pending"
            ));

        _checkoutSessionRepository
            .Setup(method => method.CreateAsync(It.IsAny<CheckoutSession>()))
            .ReturnsAsync((CheckoutSession session) => session);

        var result = await _checkoutService.CreatePixPaymentAsync(request);
        var resultObject = Assert.IsType<Result<PixPaymentResponse>>(result);

        Assert.Equal(201, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
        Assert.Equal("payt_123", resultObject.Data.PaymentId);
        Assert.Equal("QR_CODE_DATA", resultObject.Data.QrCodeData);
    }

    [Fact]
    public async Task GetPaymentLinkItemsDetailsAsync_WithInactivePaymentLink_ReturnsOkWithEmptyItems()
    {
        var paymentLinkId = "plink_123";

        _paymentLinkApiClient
            .Setup(method => method.GetAsync(paymentLinkId))
            .ReturnsAsync(new PaymentLinkHttpResponse(
                new List<PaymentLinkItemHttpResponse>(),
                false,
                true,
                "usr_123"
            ));

        var result = await _checkoutService.GetPaymentLinkItemsDetailsAsync(paymentLinkId);
        var resultObject = Assert.IsType<Result<PaymentLinkDetailsResponse>>(result);

        Assert.Equal(200, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.False(resultObject.Data.IsActive);
        Assert.Empty(resultObject.Data.items);
    }

    [Fact]
    public async Task ConfirmSandboxPaymentAsync_WithNonSandboxPaymentLink_ReturnsInternalServerError()
    {
        var paymentLinkId = "plink_123";

        _paymentLinkApiClient
            .Setup(method => method.GetAsync(paymentLinkId))
            .ReturnsAsync(new PaymentLinkHttpResponse(
                new List<PaymentLinkItemHttpResponse>(),
                true,
                false,
                "usr_123"
            ));

        var result = await _checkoutService.ConfirmSandboxPaymentAsync(paymentLinkId);
        var resultObject = Assert.IsType<Result<bool>>(result);

        Assert.Equal(500, resultObject.StatusCode);
        Assert.False(resultObject.Success);
        Assert.Equal("O link de pagamento não é sandbox", resultObject.Error!.Message);
    }

    [Fact]
    public async Task ConfirmSandboxPaymentAsync_WithValidPaymentLink_ReturnsOk()
    {
        var paymentLinkId = "plink_123";

        _paymentLinkApiClient
            .Setup(method => method.GetAsync(paymentLinkId))
            .ReturnsAsync(new PaymentLinkHttpResponse(
                new List<PaymentLinkItemHttpResponse>(),
                true,
                true,
                "usr_123"
            ));

        _paymentApiClient
            .Setup(method => method.ConfirmSandboxPaymentAsync(paymentLinkId))
            .ReturnsAsync(true);

        var result = await _checkoutService.ConfirmSandboxPaymentAsync(paymentLinkId);
        var resultObject = Assert.IsType<Result<bool>>(result);

        Assert.Equal(200, resultObject.StatusCode);
        Assert.True(resultObject.Success);
        Assert.True(resultObject.Data);
    }
}
