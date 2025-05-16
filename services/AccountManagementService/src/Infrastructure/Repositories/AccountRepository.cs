using AccountManagementService.Domain;
using AccountManagementService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementService.Infrastructure.Repositories;

public class AccountRepository(AccountDbContext context) : IAccountRepository
{
    public async Task CreateAsync(Account account)
    {
        await context.Accounts.AddAsync(account);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account account)
    {
        context.Accounts.Update(account);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Account account)
    {
        context.Accounts.Remove(account);
        await context.SaveChangesAsync();
    }
    
    public async Task<Account?> GetByIdAsync(Guid id)
        => await context.Accounts.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> AccountWithEmailExistsAsync(string email)
        => await context.Accounts.FirstOrDefaultAsync(x => x.Email == email) != null;

    public async Task<bool> AccountWithPhoneNumberExistsAsync(string phoneNumber)
        => await context.Accounts.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) != null;

    public async Task<IEnumerable<Account>> GetAllAsync()
        => await context.Accounts.ToListAsync();
}