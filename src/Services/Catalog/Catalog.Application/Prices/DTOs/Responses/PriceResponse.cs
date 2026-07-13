using Catalog.Domain.Prices.Enums;

namespace Catalog.Application.Prices.DTOs.Responses;

public record PriceResponse(
    string Id, 
    string Name, 
    string Currency, 
    decimal Amount,
    bool LiveMode, 
    bool IsActive,
    string ProductId, 
    PriceFrequency Frequency, 
    PriceCycle? Cycle,
    DateTime CreatedAt, 
    DateTime? UpdatedAt
);