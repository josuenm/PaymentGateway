using Microsoft.AspNetCore.Mvc;
using Moq;
using Payment.API.Controllers;
using Payment.Application.Payments.DTOs.Requests;
using Payment.Application.Payments.DTOs.Responses;
using Payment.Application.Payments.Interfaces;
using Payment.Application.Payments.Validators;
using Payment.Domain.Payments.Enums;
using Shared.Kernel.Results;

namespace Payment.Test.Controllers;

public class PaymentsControllerTest
{
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly PaymentsController _paymentsController;

    public PaymentsControllerTest()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _paymentsController = new PaymentsController(
            _paymentServiceMock.Object,
            new CreatePixPaymentRequestValidator()
        );
    }

    [Fact]
    public async Task CreatePixPaymentAsync_WithInvalidData_ReturnsBadRequest()
    {
        var request = new CreatePixPaymentRequest(
            new CustomerRequest("", "", "", ""),
            PaymentMethod.Pix,
            0,
            "USD",
            "",
            true
        );

        var result = await _paymentsController.CreatePixPaymentAsync(request);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(objectResult.Value);

        Assert.Equal(400, objectResult.StatusCode);
        Assert.False(resultValue.Success);

        Assert.NotNull(resultValue.Error);
        Assert.NotNull(resultValue.Error.Details);

        Assert.Equal("1 ou mais campos inválidos", resultValue.Error.Message);
        Assert.Contains("amount", resultValue.Error.Details.Keys);
    }

    [Fact]
    public async Task CreatePixPaymentAsync_WithValidData_ReturnsCreated()
    {
        var request = new CreatePixPaymentRequest(
            new CustomerRequest("cust_123", "example@example.com", "John Doe", "12345678901"),
            PaymentMethod.Pix,
            1000,
            "BRL",
            "usr_123",
            true
        );

        var paymentResponse = new PixPaymentResponse(
            "QR_CODE_DATA",
            "payt_123",
            request.Amount,
            request.Currency,
            PaymentStatus.Pending
        );

        _paymentServiceMock
            .Setup(service => service.CreatePixPaymentAsync(request))
            .ReturnsAsync(paymentResponse);

        var result = await _paymentsController.CreatePixPaymentAsync(request);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(objectResult.Value);

        Assert.Equal(201, objectResult.StatusCode);
        Assert.True(resultValue.Success);

        Assert.NotNull(resultValue.Data);
        Assert.Null(resultValue.Error);

        var data = Assert.IsType<PixPaymentResponse>(resultValue.Data);
        Assert.Equal(paymentResponse.PaymentId, data.PaymentId);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_WithValidId_ReturnsOk()
    {
        var paymentId = "payt_123";
        var paymentResponse = new PixPaymentResponse(
            "QR_CODE_DATA",
            paymentId,
            1000,
            "BRL",
            PaymentStatus.Pending
        );

        _paymentServiceMock
            .Setup(service => service.GetPixPaymentByIdAsync(paymentId))
            .ReturnsAsync(Result<PixPaymentResponse>.Ok(paymentResponse));

        var result = await _paymentsController.GetPaymentByIdAsync(paymentId);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<PixPaymentResponse>>(objectResult.Value);

        Assert.Equal(200, objectResult.StatusCode);
        Assert.True(resultValue.Success);

        Assert.NotNull(resultValue.Data);
        Assert.Null(resultValue.Error);
        Assert.Equal(paymentId, resultValue.Data.PaymentId);
    }
}
