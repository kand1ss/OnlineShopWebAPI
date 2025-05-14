using AccountManagementService.Application.Interfaces;
using AccountManagementService.Domain;
using AccountManagementService.Infrastructure.Repositories;
using Core.Contracts;
using Core.DTO;
using Core.Requests.Accounts;

namespace AccountManagementService.Application.Processors;

public class CreateAccountRequestProcessor(
    IAccountRepository repository,
    IPasswordService passwordService,
    ILogger<CreateAccountRequestProcessor> logger) 
    : IRequestProcessor<CreateAccountRequest, AccountDTO>
{
    public async Task<AccountDTO> Process(CreateAccountRequest data)
    {
        var account = new Account
        {
            Id = data.Id,
            FirstName = data.FirstName,
            LastName = data.LastName,
            Surname = data.LastName,
            PhoneNumber = data.PhoneNumber,
            Email = data.Email,
            PasswordHash = data.Password,
            DateOfBirthUtc = data.DateOfBirthUtc
        };
        
        account.PasswordHash = passwordService.HashPassword(account, data.Password);
        await repository.CreateAsync(account);
        
        logger.LogInformation($"Account with id '{account.Id}' created.");
        return account.ToDTO();
    }
}