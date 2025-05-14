
namespace CatalogManagementService.Application.DTO;

public class UpdateProductRequest(
    Guid id,
    string title,
    string? description,
    decimal price) : ProductRequest(title, description, price)
{
    public Guid Id { get; init; } = id;
}