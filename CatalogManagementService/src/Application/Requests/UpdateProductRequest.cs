using System.ComponentModel.DataAnnotations;

namespace CatalogManagementService.Application.DTO;

public record UpdateProductRequest(
    string Id,
    string Title,
    string? Description,
    decimal Price) : ProductRequest(Title, Description, Price);