using AccountManagementService.Domain;
using Core.Requests.Accounts;

namespace AccountManagementService.Application.Interfaces;

public interface IAccountUpdater
{
    void Update(Account account, UpdateAccountRequest updateData);
}