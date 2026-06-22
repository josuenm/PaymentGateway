using Catalog.Domain.Prices.Enums;

namespace Catalog.Application.Prices.DTOs.Requests;

public record CreatePriceRequest(
    string Name, 
    decimal Amount,
    string Currency, 
    PriceFrequency Frequency,
    PriceCycle? Cycle
);