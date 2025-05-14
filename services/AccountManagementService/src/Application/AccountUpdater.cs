using AccountManagementService.Application.Interfaces;
using AccountManagementService.Domain;
using Core.Requests.Accounts;

namespace AccountManagementService.Application;

public class AccountUpdater : IAccountUpdater
{
    public void Update(Account account, UpdateAccountRequest data)
    {
        if (!string.IsNullOrWhiteSpace(data.FirstName))
            account.FirstName = data.FirstName;
        if (!string.IsNullOrWhiteSpace(data.LastName))
            account.LastName = data.LastName;
        if (!string.IsNullOrWhiteSpace(data.Surname))
            account.Surname = data.Surname;
        if (!string.IsNullOrWhiteSpace(data.PhoneNumber))
            account.PhoneNumber = data.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(data.Email))
            account.Email = data.Email;
        if (data.DateOfBirth.HasValue)
            account.DateOfBirthUtc = data.DateOfBirth.Value;
    }
}