using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.DTOs.Responses;
using Checkout.Application.Checkouts.Interfaces;
using Checkout.Application.CustomerReadModels.Messaging.Commands;
using Checkout.Application.CustomerReadModels.Interfaces;
using Checkout.Application.PaymentLinkReadModels.Interfaces;
using Checkout.Application.ProductReadModels.Interfaces;
using Checkout.Domain.Checkouts.Entities;
using Checkout.Domain.Checkouts.Enums;
using Checkout.Domain.Checkouts.Repositories;
using Checkout.Domain.CustomerReadModels.Entities;
using MassTransit;
using Shared.Kernel.Results;

namespace Checkout.Application.Checkouts.Services;

public class CheckoutService : ICheckoutService
{
    private readonly ICheckoutSessionRepository _checkoutSessionRepository;
    private readonly ICustomerReadModelService _customerReadModelService;
    private readonly IPaymentLinkReadModelService _paymentLinkReadModelService;
    private readonly IPriceReadModelService _priceReadModelService;
    
    private readonly IPublishEndpoint _publishEndpoint;
    
    private readonly IPaymentApiClient _paymentApiClient;

    public CheckoutService(
        ICheckoutSessionRepository checkoutSessionRepository,
        ICustomerReadModelService customerReadModelService,
        IPaymentLinkReadModelService paymentLinkReadModelService,
        IPriceReadModelService priceReadModelService,
        
        IPublishEndpoint publishEndpoint,
        
        IPaymentApiClient paymentApiClient
    )
    {
        _checkoutSessionRepository = checkoutSessionRepository;
        _customerReadModelService = customerReadModelService;
        _paymentLinkReadModelService = paymentLinkReadModelService;
        _priceReadModelService = priceReadModelService;
        
        _publishEndpoint = publishEndpoint;
        
        _paymentApiClient = paymentApiClient;
    }
    
    public async Task<Result<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest paymentRequest)
    {
        if (paymentRequest.Method == PaymentMethod.Pix) return await CreatePixPaymentAsync(paymentRequest);

        return await CreateCreditCardPaymentAsync(paymentRequest);
    }

    public async Task<Result<PaymentResponse>> CreateCreditCardPaymentAsync(CreatePaymentRequest paymentRequest)
    {
        return Result<PaymentResponse>.Created(new PaymentResponse(
            "sess_123", 
            "payt_123" 
        ));
    }

    public async Task<Result<PaymentResponse>> CreatePixPaymentAsync(CreatePaymentRequest paymentRequest)
    {
        var paymentLink = await _paymentLinkReadModelService.GetByIdAsync(paymentRequest.SourceId);
        
        if (paymentLink == null)
            return Result<PaymentResponse>.InternalServerError("Erro ao obter o link de pagamento");

        
        if (!paymentLink.IsActive)
            return Result<PaymentResponse>.InternalServerError("O link de pagamento não esta ativo");
        
        
        var customerPayload = new CreateCustomerHttpRequest(
            paymentRequest.Customer.Email,
            paymentRequest.Customer.Name,
            paymentRequest.Customer.TaxId
        );

        
        var userId = paymentLink.UserId;
        var customer = await _customerReadModelService.GetByEmailAndUserIdAsync(userId, customerPayload.Email);

        if (customer == null)
        {
            customer = new CustomerReadModel(
                customerPayload.Email,
                customerPayload.Name,
                customerPayload.TaxId, 
                userId, 
                false
            );
            await _customerReadModelService.CreateAsync(customer);
            await _publishEndpoint.Publish(new CreateCustomerCommand(
                customer.Id,
                customer.Email,
                customer.Name,
                customer.TaxId, 
                userId,
                false
            ));
        }

        if (customer.TaxId == null || customer.Name == null)
        {
            customer.SetName(customerPayload.Name);
            customer.SetTaxId(customerPayload.TaxId);
            await _customerReadModelService.UpdateAsync(customer);
            await _publishEndpoint.Publish(new UpdateCustomerCommand(
                customer.Id,
                customer.Email,
                customer.Name,
                customer.TaxId, 
                userId,
                false
            ));
        }
        
        var priceIdList = paymentLink
            .Items
            .ToList()
            .Select(p => p.PriceId)
            .ToList();

        var prices = await _priceReadModelService.GetManyByIdAsync(priceIdList);

        var total = prices.Sum(p => p.Amount);
        
        var paymentPayload = new CreatePixPaymentHttpRequest(
            new CustomerPixPaymentHttpRequest(
                customer.Id, 
                customer.Email, 
                customer.Name!, 
                customer.TaxId!
            ), 
            paymentRequest.Method, 
            total, 
            "BRL", 
            userId, 
            true 
        );
        var payment = await _paymentApiClient.CreatePixPaymentAsync(paymentPayload);
        
        if (payment == null)
            return Result<PaymentResponse>.InternalServerError("Erro ao gerar cobrança");
        
        var paymentId = payment.PaymentId;
        var qrCodeData = payment.QrCodeData;

        var session = new CheckoutSession(
            customer.Id, 
            paymentId, 
            paymentRequest.SourceId, 
            total, 
        "BRL", 
            userId, 
            paymentRequest.Method
        );
        
        await _checkoutSessionRepository.CreateAsync(session);

        return Result<PaymentResponse>.Created(new PaymentResponse(
            session.Id, 
            paymentId, 
            qrCodeData, 
            13000, 
            total
        ));
    }
    
    public async Task<Result<PaymentLinkDetailsResponse>> GetPaymentLinkItemsDetailsAsync(string paymentLinkId)
    {
        try
        {
            var paymentLink = await _paymentLinkReadModelService.GetByIdAsync(paymentLinkId);

            if (paymentLink == null)
                return Result<PaymentLinkDetailsResponse>.InternalServerError("Erro ao obter o link de pagamento");

            if (!paymentLink.IsActive)
                return Result<PaymentLinkDetailsResponse>
                    .Ok(new PaymentLinkDetailsResponse(
                        new List<ItemResponse>(), 
                        paymentLink.IsActive, 
                        paymentLink.LiveMode
                        )
                    );

            var paymentLinkItems = paymentLink.Items.ToList();
            
            var priceIdList = paymentLinkItems
                .Select(p => p.PriceId)
                .ToList();
            
            var prices = await _priceReadModelService.GetManyByIdAsync(priceIdList);
            
            var items = prices.Select(p =>
            {
                var item = paymentLinkItems.FirstOrDefault(i =>
                {
                    Console.WriteLine($"PAYMENT LINK ITEM ID: {i.PriceId} | PRICE ID: {p.Id}");
                    return i.PriceId == p.Id;
                });

                if (item == null) return null;
                
                return new ItemResponse(
                    p.ProductId,
                    p.Id,
                    p.Name,
                    item.Quantity,
                    p.Currency,
                    p.Amount, 
                    p.Frequency,
                    p.Cycle
                );
            })
            .Where(p => p != null)
            .Select(p => p!)
            .ToList();
            
            return Result<PaymentLinkDetailsResponse>.Ok(new PaymentLinkDetailsResponse(
                items, 
                paymentLink.IsActive, 
                paymentLink.LiveMode
            ));
        }
        catch (HttpRequestException e)
        {
            return Result<PaymentLinkDetailsResponse>.BadRequest("Erro ao obter o link de pagamento");
        }
    }
    
    public async Task<Result<bool>> ConfirmSandboxPaymentAsync(string paymentLinkId)
    {
        try
        {
            var paymentLink = await _paymentLinkReadModelService.GetByIdAsync(paymentLinkId);

            if (paymentLink == null)
                return Result<bool>.InternalServerError("Erro ao obter o link de pagamento");

            if (!paymentLink.LiveMode)
                return Result<bool>.InternalServerError("O link de pagamento não é sandbox");

            var confirmation = await _paymentApiClient.ConfirmSandboxPaymentAsync(paymentLinkId);
            
            if (confirmation == null)
                return Result<bool>.InternalServerError("Erro ao confirmar pagamento");
            
            return Result<bool>.Ok(confirmation.Value);
        }
        catch (HttpRequestException e)
        {
            return Result<bool>.BadRequest("Erro ao confirmar pagamento");
        }
    }
}