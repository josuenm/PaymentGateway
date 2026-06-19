using Catalog.Application.Prices.DTOs.Requests;

namespace Catalog.Application.Products.DTOs.Requests;

public record CreateProductRequest(
    string Name, 
    string? Description, 
    IEnumerable<CreatePriceRequest>? Prices, 
    bool IsActive = true, 
    object? Metadata = null
);