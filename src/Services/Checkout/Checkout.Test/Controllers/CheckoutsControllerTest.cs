using Checkout.API.Controllers;
using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.DTOs.Responses;
using Checkout.Application.Checkouts.Interfaces;
using Checkout.Application.Checkouts.Validators;
using Checkout.Domain.Checkouts.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Kernel.Results;

namespace Checkout.Test.Controllers;

public class CheckoutsControllerTest
{
    
    private readonly Mock<ICheckoutService> _checkoutServiceMock;
    private readonly CheckoutsController _checkoutsController;

    public CheckoutsControllerTest()
    {
        _checkoutServiceMock = new Mock<ICheckoutService>();
        _checkoutsController = new CheckoutsController(
            _checkoutServiceMock.Object, 
            new CreatePaymentRequestValidator()
        );
    }
    
    [Fact]
    public async Task CreatePayment_InvalidData_ReturnsBadRequest()
    {
        var request = new CreatePaymentRequest(PaymentMethod.Pix, "pl_123", new CustomerPaymentRequest(
            "John Doe", 
            "example@gmail.com", 
            "", 
            null
        ));
        
        var result = await _checkoutsController.CreatePaymentAsync(request);
        var resultObject = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(resultObject.Value);
        
        Assert.Equal(400, resultObject.StatusCode);
        
        Assert.Null(resultValue.Data);
        Assert.False(resultValue.Success);
        Assert.NotNull(resultValue.Error);
        Assert.NotNull(resultValue.Error.Details);
        
        Assert.Equal("1 ou mais campos inválidos", resultValue.Error.Message);
        Assert.Contains("taxId", resultValue.Error.Details.Keys);
    }

    [Fact]
    public async Task CreatePaymentAsync_WithValidData_ReturnsCreated()
    {
        var request = new CreatePaymentRequest(
            PaymentMethod.Pix,
            "plink_123",
            new CustomerPaymentRequest("John Doe", "example@example.com", "12345678901", null)
        );

        var paymentResponse = new PaymentResponse(
            "cs_123",
            "payt_123",
            "QR_CODE_DATA",
            13000,
            1000
        );

        _checkoutServiceMock
            .Setup(service => service.CreatePixPaymentAsync(request))
            .ReturnsAsync(Result<PaymentResponse>.Created(paymentResponse));

        var result = await _checkoutsController.CreatePaymentAsync(request);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<PaymentResponse>>(objectResult.Value);

        Assert.Equal(201, objectResult.StatusCode);
        Assert.True(resultValue.Success);

        Assert.NotNull(resultValue.Data);
        Assert.Null(resultValue.Error);
        Assert.Equal(paymentResponse.PaymentId, resultValue.Data.PaymentId);
    }

    [Fact]
    public async Task ConfirmSandboxPaymentAsync_WithValidId_ReturnsOk()
    {
        var paymentId = "payt_123";

        _checkoutServiceMock
            .Setup(service => service.ConfirmSandboxPaymentAsync(paymentId))
            .ReturnsAsync(Result<bool>.Ok(true));

        var result = await _checkoutsController.ConfirmSandboxPaymentAsync(paymentId);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<bool>>(objectResult.Value);

        Assert.Equal(200, objectResult.StatusCode);
        Assert.True(resultValue.Success);
        Assert.True(resultValue.Data);
    }

    [Fact]
    public async Task GetItemsDetailsAsync_WithValidId_ReturnsOk()
    {
        var paymentLinkId = "plink_123";
        var detailsResponse = new PaymentLinkDetailsResponse(
            new List<ItemResponse>(),
            true,
            true
        );

        _checkoutServiceMock
            .Setup(service => service.GetPaymentLinkItemsDetailsAsync(paymentLinkId))
            .ReturnsAsync(Result<PaymentLinkDetailsResponse>.Ok(detailsResponse));

        var result = await _checkoutsController.GetItemsDetailsAsync(paymentLinkId);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<PaymentLinkDetailsResponse>>(objectResult.Value);

        Assert.Equal(200, objectResult.StatusCode);
        Assert.True(resultValue.Success);

        Assert.NotNull(resultValue.Data);
        Assert.True(resultValue.Data.IsActive);
    }
}
