namespace Catalog.Application.Products.DTOs.Responses;

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int Total,
    int Page,
    int TotalPages,
    int Limit
);