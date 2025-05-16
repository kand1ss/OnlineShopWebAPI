using AccountManagementService.Application.Interfaces;
using AccountManagementService.Infrastructure.Repositories;
using Core.Contracts;
using Core.DTO;
using Core.Requests.Accounts;

namespace AccountManagementService.Application.Processors;

public class UpdateAccountRequestProcessor(
    IAccountRepository repository,
    IAccountUpdater accountUpdater,
    IPasswordService passwordService,
    ILogger<UpdateAccountRequestProcessor> logger) : IRequestProcessor<UpdateAccountRequest, AccountDTO>
{
    public async Task<AccountDTO> Process(UpdateAccountRequest data)
    {
        var account = await repository.GetByIdAsync(data.Id) 
                      ?? throw new InvalidOperationException($"UPDATE: Account with id '{data.Id}' not found.");
        
        if (!string.IsNullOrEmpty(data.Email))
            if (!await repository.AccountWithEmailExistsAsync(data.Email))
                throw new InvalidOperationException($"UPDATE: Account with email '{data.Email}' already exists.");
        
        if (!string.IsNullOrEmpty(data.PhoneNumber))
            if (!await repository.AccountWithPhoneNumberExistsAsync(data.PhoneNumber))
                throw new InvalidOperationException($"UPDATE: Account with phone number '{data.PhoneNumber}' already exists.");
        
        
        accountUpdater.Update(account, data);
        if (!string.IsNullOrWhiteSpace(data.Password))
            account.PasswordHash = passwordService.HashPassword(account, data.Password);

        await repository.UpdateAsync(account);
        logger.LogInformation($"Account with id '{account.Id}' updated.");
        return account.ToDTO();
    }
}