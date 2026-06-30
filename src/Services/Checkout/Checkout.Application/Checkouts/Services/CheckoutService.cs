using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Application.Checkouts.DTOs.Responses;
using Checkout.Application.Checkouts.Interfaces;
using Checkout.Domain.Checkouts.Entities;
using Checkout.Domain.Checkouts.Repositories;
using Checkout.Infrastructure.Http.Interfaces;
using Shared.Kernel.Results;

namespace Checkout.Application.Checkouts.Services;

public class CheckoutService : ICheckoutService
{
    private readonly ICheckoutSessionRepository _checkoutSessionRepository;
    private readonly ICustomerApiClient _customerApiClient;
    private readonly IPaymentLinkApiClient _paymentLinkApiClient;
    private readonly IPaymentApiClient _paymentApiClient;
    private readonly IPriceApiClient _priceApiClient;

    public CheckoutService(
        ICheckoutSessionRepository checkoutSessionRepository, 
        ICustomerApiClient customerApiClient,
        IPaymentLinkApiClient paymentLinkApiClient, 
        IPaymentApiClient paymentApiClient,
        IPriceApiClient priceApiClient
    )
    {
        _checkoutSessionRepository = checkoutSessionRepository;
        
        _customerApiClient = customerApiClient;
        _paymentLinkApiClient = paymentLinkApiClient;
        _paymentApiClient = paymentApiClient;
        _priceApiClient = priceApiClient;
    }

    public async Task<Result<PixPaymentResponse>> CreatePixPaymentAsync(CreatePaymentRequest paymentRequest)
    {
        var paymentLink = await _paymentLinkApiClient.GetAsync(paymentRequest.SourceId);
        
        if (paymentLink == null)
            return Result<PixPaymentResponse>.InternalServerError("Erro ao obter o link de pagamento");

        
        if (!paymentLink.IsActive)
            return Result<PixPaymentResponse>.InternalServerError("O link de pagamento não esta ativo");
        
        
        var customerPayload = new CreateCustomerHttpRequest(
            paymentRequest.Customer.Email,
            paymentRequest.Customer.Name,
            paymentRequest.Customer.TaxId
        );

        
        var userId = paymentLink.UserId;
        var customer = await _customerApiClient.GetOrCreateAsync(userId, customerPayload);

        
        if (customer == null)
            return Result<PixPaymentResponse>.InternalServerError("Erro ao obter o cliente");
        
        
        
        
        var priceIdList = paymentLink
            .Items
            .ToList()
            .Select(p => p.PriceId)
            .ToList();
            
        var prices = await _priceApiClient.GetManyByIdAsync(priceIdList);
            
        if (prices == null)
            return Result<PixPaymentResponse>.InternalServerError("Erro ao obter itens");

        
        var total = prices.Sum(p => p.Amount);
        
        
        
        
        var paymentPayload = new CreatePixPaymentHttpRequest(
            new CustomerPixPaymentHttpRequest(
                customer.Id, 
                customer.Email, 
                customer.Name, 
                customer.TaxId
            ), 
            paymentRequest.Method, 
            total, 
            "BRL", 
            userId, 
            true 
        );
        var payment = await _paymentApiClient.CreatePixPaymentAsync(paymentPayload);
        
        if (payment == null)
            return Result<PixPaymentResponse>.InternalServerError("Erro ao gerar cobrança");
        
        
        
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

        return Result<PixPaymentResponse>.Created(new PixPaymentResponse(
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
            var paymentLink = await _paymentLinkApiClient.GetAsync(paymentLinkId);

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
            
            var prices = await _priceApiClient.GetManyByIdAsync(priceIdList);
            
            if (prices == null)
                return Result<PaymentLinkDetailsResponse>.InternalServerError("Erro ao obter itens");
            
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
            var paymentLink = await _paymentLinkApiClient.GetAsync(paymentLinkId);

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