using AccountManagementService.Domain;

namespace AccountManagementService.Infrastructure.Repositories;

public interface IAccountRepository
{
    Task CreateAsync(Account account);
    Task UpdateAsync(Account account);
    Task DeleteAsync(Account account);
    Task<Account?> GetByIdAsync(Guid id);
    Task<IEnumerable<Account>> GetAllAsync();
}