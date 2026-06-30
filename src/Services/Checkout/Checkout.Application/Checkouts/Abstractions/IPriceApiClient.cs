using Checkout.Application.Checkouts.DTOs.Responses;

namespace Checkout.Infrastructure.Http.Interfaces;

public interface IPriceApiClient
{
    public Task<IEnumerable<PriceHttpResponse>?> GetManyByIdAsync(IEnumerable<string> idList);
}