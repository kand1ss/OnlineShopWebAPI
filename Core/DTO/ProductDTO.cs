using System.ComponentModel.DataAnnotations;

namespace Core.DTO;

public record ProductDTO(
    Guid Id,
    string Title,
    string? Description,
    decimal Price);