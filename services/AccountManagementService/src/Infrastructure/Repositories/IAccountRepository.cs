using AccountManagementService.Domain;

namespace AccountManagementService.Infrastructure.Repositories;

public interface IAccountRepository
{
    Task CreateAsync(Account account);
    Task UpdateAsync(Account account);
    Task DeleteAsync(Account account);
    Task<Account?> GetByIdAsync(Guid id);
    Task<bool> AccountWithEmailExistsAsync(string email);
    Task<bool> AccountWithPhoneNumberExistsAsync(string phoneNumber);
    Task<IEnumerable<Account>> GetAllAsync();
}