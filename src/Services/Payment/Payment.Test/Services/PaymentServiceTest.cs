using System.Text.Json;
using Moq;
using Payment.Application.Payments.DTOs.Requests;
using Payment.Application.Payments.DTOs.Responses;
using Payment.Application.Payments.Services;
using Payment.Domain.Payments.Entities;
using Payment.Domain.Payments.Enums;
using Payment.Domain.Payments.Repositories;
using Shared.Kernel.Results;

namespace Payment.Test.Services;

public class PaymentServiceTest
{
    private readonly PaymentService _paymentService;
    private readonly Mock<IPaymentTransactionRepository> _paymentTransactionRepository;

    public PaymentServiceTest()
    {
        _paymentTransactionRepository = new Mock<IPaymentTransactionRepository>();
        _paymentService = new PaymentService(_paymentTransactionRepository.Object);
    }

    [Fact]
    public async Task CreatePixPaymentAsync_WithValidData_ReturnsPixPaymentResponse()
    {
        var request = new CreatePaymentRequest(
            new CustomerRequest("cust_123", "example@example.com", "John Doe", "12345678901"),
            PaymentMethod.Pix,
            1000,
            "BRL",
            "usr_123",
            true
        );

        _paymentTransactionRepository
            .Setup(method => method.CreateAsync(It.IsAny<PaymentTransactionEntity>()))
            .ReturnsAsync((PaymentTransactionEntity payment) => payment);

        var result = await _paymentService.CreatePaymentAsync(request);

        Assert.Equal(request.Amount, result.Amount);
        Assert.Equal(request.Currency, result.Currency);
        Assert.Equal(PaymentStatus.Pending, result.Status);
    }

    [Fact]
    public async Task ConfirmSandboxPaymentAsync_WithInvalidId_ReturnsNotFound()
    {
        var paymentId = "payt_123";

        _paymentTransactionRepository
            .Setup(method => method.GetByIdAsync(paymentId))
            .ReturnsAsync((PaymentTransactionEntity?)null);

        var result = await _paymentService.ConfirmSandboxPaymentAsync(paymentId);
        var resultObject = Assert.IsType<Result<bool>>(result);

        Assert.Equal(404, resultObject.StatusCode);
        Assert.False(resultObject.Success);
        Assert.Equal("Pagamento não encontrado", resultObject.Error!.Message);
    }

    [Fact]
    public async Task ConfirmSandboxPaymentAsync_WithNonSandboxPayment_ReturnsBadRequest()
    {
        var chargeResponse = new PixAcquirerResponse("charge_123", "QR_CODE_DATA");
        var payment = new PaymentTransactionEntity(
            "payt_123",
            "cust_123",
            PaymentMethod.Pix,
            PaymentStatus.Pending,
            1000,
            "BRL",
            "usr_123",
            chargeResponse.Id,
            JsonSerializer.Serialize(chargeResponse),
            false
        );

        _paymentTransactionRepository
            .Setup(method => method.GetByIdAsync(payment.Id))
            .ReturnsAsync(payment);

        var result = await _paymentService.ConfirmSandboxPaymentAsync(payment.Id);
        var resultObject = Assert.IsType<Result<bool>>(result);

        Assert.Equal(400, resultObject.StatusCode);
        Assert.False(resultObject.Success);
        Assert.Equal("O pagamento não é sandbox", resultObject.Error!.Message);
    }

    [Fact]
    public async Task ConfirmSandboxPaymentAsync_WithValidPayment_ReturnsOk()
    {
        var chargeResponse = new PixAcquirerResponse("charge_123", "QR_CODE_DATA");
        var payment = new PaymentTransactionEntity(
            "payt_123",
            "cust_123",
            PaymentMethod.Pix,
            PaymentStatus.Pending,
            1000,
            "BRL",
            "usr_123",
            chargeResponse.Id,
            JsonSerializer.Serialize(chargeResponse),
            true
        );

        _paymentTransactionRepository
            .Setup(method => method.GetByIdAsync(payment.Id))
            .ReturnsAsync(payment);

        _paymentTransactionRepository
            .Setup(method => method.SetPaidAsync(payment))
            .ReturnsAsync(true);

        var result = await _paymentService.ConfirmSandboxPaymentAsync(payment.Id);
        var resultObject = Assert.IsType<Result<bool>>(result);

        Assert.Equal(200, resultObject.StatusCode);
        Assert.True(resultObject.Success);
        Assert.True(resultObject.Data);
    }
}
