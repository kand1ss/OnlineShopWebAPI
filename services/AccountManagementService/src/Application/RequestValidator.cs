using AccountManagementService.Infrastructure.Repositories;

namespace AccountManagementService.Application;

public class RequestValidator(IAccountRepository accountRepository)
{
    public async Task<bool> Validate(string email, string phoneNumber)
        => !await accountRepository.AccountWithEmailExistsAsync(email)
           || !await accountRepository.AccountWithPhoneNumberExistsAsync(phoneNumber);
}