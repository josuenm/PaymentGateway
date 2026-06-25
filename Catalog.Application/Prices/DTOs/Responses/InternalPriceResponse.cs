using Catalog.Domain.Prices.Enums;

namespace Catalog.Application.Prices.DTOs.Responses;

public record InternalPriceResponse(
    string Id, 
    string Name, 
    long Amount, 
    string Currency, 
    string ProductId, 
    bool IsActive,
    PriceFrequency Frequency, 
    PriceCycle? Cycle
);