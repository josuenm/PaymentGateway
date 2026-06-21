using Catalog.Application.Prices.DTOs.Responses;
using Catalog.Application.Products.DTOs.Requests;
using Catalog.Application.Products.DTOs.Responses;
using Catalog.Application.Products.Interfaces;
using Catalog.Domain.Prices.Entities;
using Catalog.Domain.Products.Entities;
using Catalog.Domain.Products.Repositories;
using Customers.Application.Customers.DTOs.Responses;
using MassTransit;
using Shared.IntegrationEvents;
using Shared.Kernel.Results;

namespace Catalog.Application.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ProductService(
        IProductRepository productRepository, 
        IPublishEndpoint publishEndpoint
    )
    {
        _productRepository = productRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<ProductResponse>> CreateAsync(string userId, CreateProductRequest request)
    {
        var product = new Product(
            request.Name, 
            userId,
            true, 
            true, 
            request.Description, 
            request.Metadata
        );

        if (request.Prices != null && request.Prices.Any())
        {
            var prices = request.Prices.Select(item => new Price(
                item.Name, 
                item.Amount, 
                item.Currency, 
                item.Frequency, 
                item.Cycle, 
                product.Id, 
                userId
            ));
            
            product.SetPrices(prices);
        }
        
        await _productRepository.CreateAsync(product);

        await _publishEndpoint.Publish(new ProductCreatedEvent(
            product.Id, 
            product.Name, 
            product.Description, 
            product.LiveMode, 
            product.IsActive,
            product.UserId,
            product.Prices != null && product.Prices.Any() ? product.Prices.Select(price => new PriceCreatedEvent(
                price.Id, 
                price.Name, 
                price.AmountInCents, 
                price.Currency, 
                price.IsActive, 
                price.LiveMode, 
                price.Frequency.ToString(), 
                price.Cycle?.ToString() ?? null
            )) : new List<PriceCreatedEvent>(),
            product.CreatedAt
        ));

        return Result<ProductResponse>.Created(new ProductResponse(
            product.Id, 
            product.Name, 
            product.Description, 
            product.LiveMode, 
            product.IsActive, 
            product.UserId, 
            product.Prices != null && product.Prices.Any() ? product.Prices.Select(item => new PriceResponse(
                item.Id, 
                item.Name, 
                item.Currency, 
                item.AmountInCents,
                item.LiveMode,
                item.IsActive,
                item.ProductId, 
                item.UserId,
                item.Frequency, 
                item.Cycle,
                item.CreatedAt, 
                item.UpdatedAt
            )) : new List<PriceResponse>(),
            product.Metadata, 
            product.CreatedAt, 
            product.UpdatedAt
        ));
    }

    public async Task<Result<ProductResponse>> GetByIdAsync(string userId, string id)
    {
        var product = await _productRepository.GetByIdAsync(userId, id);

        if (product == null)
            return Result<ProductResponse>.Failure("O produto não foi encontrado", ErrorType.NotFound);

        return Result<ProductResponse>.Ok(new ProductResponse(
            product.Id, 
            product.Name, 
            product.Description, 
            product.LiveMode, 
            product.IsActive, 
            product.UserId, 
            product.Prices != null && product.Prices.Any() ? product.Prices.Select(item => new PriceResponse(
                item.Id, 
                item.Name, 
                item.Currency, 
                item.AmountInCents,
                item.LiveMode,
                item.IsActive,
                item.ProductId, 
                item.UserId,
                item.Frequency, 
                item.Cycle,
                item.CreatedAt, 
                item.UpdatedAt
            )) : new List<PriceResponse>(),
            product.Metadata, 
            product.CreatedAt, 
            product.UpdatedAt
        ));
    }

    public async Task<PagedResponse<ProductResponse>> GetAllPagedAsync(string userId, int page, int limit)
    {
        var result = await _productRepository.GetAllPagedAsync(userId, page, limit);

        var products = result.Items.Select(item => new ProductResponse(
            item.Id,
            item.Name,
            item.Description,
            item.LiveMode,
            item.IsActive,
            item.UserId, 
            item.Prices != null && item.Prices.Any() ? item.Prices.Select(price => new PriceResponse(
                price.Id, 
                price.Name, 
                price.Currency, 
                price.AmountInCents,
                price.LiveMode,
                price.IsActive,
                price.ProductId, 
                price.UserId,
                price.Frequency, 
                price.Cycle,
                price.CreatedAt, 
                price.UpdatedAt
            )) : new List<PriceResponse>(),
            item.Metadata,
            item.CreatedAt,
            item.UpdatedAt
        ));

        return new PagedResponse<ProductResponse>(
            products, 
            result.Total, 
            page, 
            (int)Math.Ceiling((double)result.Total / limit),
            limit
        );
    }
}