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
        var request = new CreatePaymentRequest(
            new CustomerRequest("", "", "", ""),
            PaymentMethod.Pix,
            0,
            "USD",
            "",
            true
        );

        var result = await _paymentsController.CreatePaymentAsync(request);
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
        var request = new CreatePaymentRequest(
            new CustomerRequest("cust_123", "example@example.com", "John Doe", "12345678901"),
            PaymentMethod.Pix,
            1000,
            "BRL",
            "usr_123",
            true
        );

        var paymentResponse = new PaymentResponse(
            "payt_123",
            request.Amount,
            request.Currency,
            PaymentStatus.Pending,
            "QR_CODE_DATA"
        );

        _paymentServiceMock
            .Setup(service => service.CreatePaymentAsync(request))
            .ReturnsAsync(paymentResponse);

        var result = await _paymentsController.CreatePaymentAsync(request);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(objectResult.Value);

        Assert.Equal(201, objectResult.StatusCode);
        Assert.True(resultValue.Success);

        Assert.NotNull(resultValue.Data);
        Assert.Null(resultValue.Error);

        var data = Assert.IsType<PaymentResponse>(resultValue.Data);
        Assert.Equal(paymentResponse.PaymentId, data.PaymentId);
    }
}
