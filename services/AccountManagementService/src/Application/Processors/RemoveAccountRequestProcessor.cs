using AccountManagementService.Infrastructure.Repositories;
using Core.Contracts;
using Core.DTO;
using Core.Requests.Accounts;

namespace AccountManagementService.Application.Processors;

public class RemoveAccountRequestProcessor(
    IAccountRepository repository) : IRequestProcessor<RemoveAccountRequest, AccountDTO>
{
    public async Task<AccountDTO> Process(RemoveAccountRequest data)
    {
        var account = await repository.GetByIdAsync(data.Id)
            ?? throw new InvalidOperationException($"DELETE: Account with id '{data.Id}' not found.");
        
        await repository.DeleteAsync(account);
        return account.ToDTO();
    }
}