using Catalog.Application.Products.DTOs.Responses;
using Catalog.Application.Products.DTOs.Requests;
using Shared.DTOs.Responses;
using Shared.Kernel.Results;

namespace Catalog.Application.Products.Interfaces;

public interface IProductService
{
    public Task<Result<ProductResponse>> CreateAsync(string userId, CreateProductRequest request);
    public Task<Result<ProductResponse>> GetByIdAsync(string userId, string productId);
    public Task<PagedResponse<ProductResponse>> GetAllPagedAsync(string userId, int page, int limit);
}