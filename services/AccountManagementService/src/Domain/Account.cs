using Core.DTO;

namespace AccountManagementService.Domain;

public class Account : AccountDTO
{
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; init; } = DateTime.UtcNow;
}