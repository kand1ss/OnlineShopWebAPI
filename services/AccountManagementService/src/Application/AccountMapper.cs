using AccountManagementService.Domain;
using Core.DTO;

namespace AccountManagementService.Application;

public static class AccountMapper
{
    public static AccountDTO ToDTO(this Account account)
        => new()
        {
            Id = account.Id,
            FirstName = account.FirstName,
            LastName = account.LastName,
            Surname = account.Surname,
            PhoneNumber = account.PhoneNumber,
            Email = account.Email,
            DateOfBirthUtc = account.DateOfBirthUtc,
            PasswordHash = account.PasswordHash
        };

    public static Account ToDomain(this AccountDTO account)
        => new()
        {
            Id = account.Id,
            FirstName = account.FirstName,
            LastName = account.LastName,
            Surname = account.Surname,
            PhoneNumber = account.PhoneNumber,
            Email = account.Email,
            DateOfBirthUtc = account.DateOfBirthUtc,
            PasswordHash = account.PasswordHash
        };
}