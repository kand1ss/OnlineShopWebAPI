
namespace CatalogManagementService.Application.DTO;

public class CreateProductRequest(
    string title,
    string? description,
    decimal price) : ProductRequest(title, description, price);