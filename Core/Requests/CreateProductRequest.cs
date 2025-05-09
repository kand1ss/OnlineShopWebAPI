namespace CatalogManagementService.Application.DTO;

public record CreateProductRequest(
    string Title,
    string? Description,
    decimal Price) : ProductRequest(Title, Description, Price);