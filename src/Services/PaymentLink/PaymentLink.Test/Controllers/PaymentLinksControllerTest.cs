using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentLink.API.Controllers;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;
using PaymentLink.Application.PaymentLinks.DTOs.Responses;
using PaymentLink.Application.PaymentLinks.Interfaces;
using PaymentLink.Application.PaymentLinks.Validators;
using PaymentLink.Domain.PaymentLinks.Enums;
using Shared.Kernel.Results;

namespace PaymentLink.Test.Controllers;

public class PaymentLinksControllerTest
{
    private readonly Mock<IPaymentLinkService> _paymentLinkServiceMock;
    private readonly PaymentLinksController _paymentLinksController;

    public PaymentLinksControllerTest()
    {
        _paymentLinkServiceMock = new Mock<IPaymentLinkService>();
        _paymentLinksController = new PaymentLinksController(
            _paymentLinkServiceMock.Object,
            new CreatePaymentLinkValidator()
        );
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ReturnsBadRequest()
    {
        var request = new CreatePaymentLink(
            null!,
            new List<CreatePaymentLinkItem> { new("", 0) }
        );

        var result = await _paymentLinksController.CreateAsync(request, "usr_123");
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(objectResult.Value);

        Assert.Equal(400, objectResult.StatusCode);
        Assert.False(resultValue.Success);

        Assert.NotNull(resultValue.Error);
        Assert.NotNull(resultValue.Error.Details);

        Assert.Equal("1 ou mais campos inválidos", resultValue.Error.Message);
        Assert.Contains("methods", resultValue.Error.Details.Keys);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreated()
    {
        var request = new CreatePaymentLink(
            new List<PaymentLinkMethods> { PaymentLinkMethods.Pix },
            new List<CreatePaymentLinkItem> { new("pri_123", 1) }
        );

        _paymentLinkServiceMock
            .Setup(service => service.CreateAsync(request, "usr_123"))
            .ReturnsAsync(Result<PaymentLinkResponse>.Created(new PaymentLinkResponse(
                "plink_123",
                true,
                true,
                request.Methods,
                new List<PaymentLinkItemResponse>(),
                DateTime.UtcNow,
                null
            )));

        var result = await _paymentLinksController.CreateAsync(request, "usr_123");
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<PaymentLinkResponse>>(objectResult.Value);

        Assert.Equal(201, objectResult.StatusCode);
        Assert.True(resultValue.Success);

        Assert.NotNull(resultValue.Data);
        Assert.Null(resultValue.Error);
    }
}
