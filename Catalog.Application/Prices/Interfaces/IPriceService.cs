using Catalog.Application.Prices.DTOs.Responses;
using Shared.Kernel.Results;

namespace Catalog.Application.Prices.Interfaces;

public interface IPriceService
{
    public Task<Result<IEnumerable<InternalPriceResponse>>> InternalGetManyByIdAsync(IEnumerable<string> idList);
}