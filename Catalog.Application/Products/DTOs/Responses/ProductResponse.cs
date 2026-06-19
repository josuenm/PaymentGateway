namespace Catalog.Application.Products.DTOs.Responses;

public record ProductResponse(
    string Id, 
    string Name, 
    string? Description,
    bool LiveMode,
    bool IsActive,
    string UserId, 
    object? Metadata, 
    DateTime CreatedAt, 
    DateTime? UpdatedAt = null
);