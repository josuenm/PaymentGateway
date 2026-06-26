using Catalog.Domain.Prices.Enums;

namespace Catalog.Application.Prices.DTOs.Requests;

public record CreatePriceRequest(
    string Name, 
    long Amount,
    string Currency, 
    PriceFrequency Frequency,
    PriceCycle? Cycle
);