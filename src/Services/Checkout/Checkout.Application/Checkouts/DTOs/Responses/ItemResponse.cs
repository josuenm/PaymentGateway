using Checkout.Domain.ProductReadModels.Enums;

namespace Checkout.Application.Checkouts.DTOs.Responses;

public record ItemResponse(
    string ProductId,
    string PriceId,
    string Name,
    int Quantity,
    string Currency,
    long Amount,
    PriceReadModelFrequency Frequency, 
    PriceReadModelCycle? Cycle 
);