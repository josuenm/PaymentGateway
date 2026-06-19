using Catalog.Application.Products.DTOs.Requests;
using Catalog.Application.Products.DTOs.Responses;
using Catalog.Application.Products.Interfaces;
using Catalog.Domain.Products.Entities;
using Catalog.Domain.Products.Repositories;
using Customers.Application.Customers.DTOs.Responses;
using Shared.Kernel.Results;

namespace Catalog.Application.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductResponse>> CreateAsync(string userId, CreateProductRequest request)
    {
        var productFound = await _productRepository.GetByIdAsync(userId, request.Name);

        if (productFound != null)
            return Result<ProductResponse>.Failure(
                "Já existe um produto com o mesmo nome", 
                ErrorType.Conflict);

        var product = new Product(
            request.Name, 
            userId,
            true, 
            true, 
            request.Description, 
            request.Metadata
        );
        
        var createdProduct = await _productRepository.CreateAsync(product);

        return Result<ProductResponse>.Created(new ProductResponse(
            product.Id, 
            product.Name, 
            product.Description, 
            product.LiveMode, 
            product.IsActive, 
            product.UserId, 
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