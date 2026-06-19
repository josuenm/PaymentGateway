namespace Customers.Domain.Commons;

public record PagedSearchResult<T>(
    IEnumerable<T> Items,
    int Total
);