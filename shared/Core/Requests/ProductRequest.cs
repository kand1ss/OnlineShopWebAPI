using System.ComponentModel.DataAnnotations;

namespace CatalogManagementService.Application.DTO;

public class ProductRequest(string title, string? description, decimal price)
{
    [MaxLength(100)]
    [MinLength(5)]
    [Required]
    public string Title { get; init; } = title;

    [MaxLength(2500)]
    public string? Description { get; init; } = description;

    [Required] 
    [Range(1, 1000000000)]
    public decimal Price { get; init; } = price;
}