
namespace CatalogManagementService.Application.DTO;

public record UpdateProductRequest(
    Guid Id,
    string Title,
    string? Description,
    decimal Price) : ProductRequest(Title, Description, Price);