using System.ComponentModel.DataAnnotations;

namespace CatalogManagementService.Application.DTO;

public record ProductRequest(
    [MaxLength(100)]
    [MinLength(10)]
    [Required]
    string Title,
    [MaxLength(2500)]
    string? Description,
    [Required]
    decimal Price);