using Catalog.Application.Prices.DTOs.Responses;

namespace Catalog.Application.Products.DTOs.Responses;

public record ProductResponse(
    string Id, 
    string Name, 
    string? Description,
    bool LiveMode,
    bool IsActive,
    string UserId,
    IEnumerable<PriceResponse> Prices,
    object? Metadata, 
    DateTime CreatedAt, 
    DateTime? UpdatedAt = null
);