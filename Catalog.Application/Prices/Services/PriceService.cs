using Catalog.Application.Prices.DTOs.Responses;
using Catalog.Application.Prices.Interfaces;
using Catalog.Domain.Prices.Repositories;
using Shared.Kernel.Results;

namespace Catalog.Application.Prices.Services;

public class PriceService : IPriceService
{
    private readonly IPriceRepository _priceRepository;

    public PriceService(IPriceRepository priceRepository)
    {
        _priceRepository = priceRepository;
    }

    public async Task<Result<IEnumerable<InternalPriceResponse>>> InternalGetManyByIdAsync(IEnumerable<string> idList)
    {
        var prices = (await _priceRepository.GetManyByIdAsync(
            idList,
            true,
            true
        )).ToList();

        return Result<IEnumerable<InternalPriceResponse>>.Ok(prices.Select(p => new InternalPriceResponse(
            p.Id, 
            p.Name, 
            p.AmountInCents, 
            p.Currency,
            p.ProductId,
            p.IsActive,
            p.Frequency, 
            p.Cycle
        )));
    }
}