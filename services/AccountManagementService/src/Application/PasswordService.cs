using AccountManagementService.Application.Interfaces;
using AccountManagementService.Domain;
using Microsoft.AspNetCore.Identity;

namespace AccountManagementService.Application;

public class PasswordService : IPasswordService
{
    public string HashPassword(Account account, string password)
    {
        var passwordHasher = new PasswordHasher<Account>();
        return passwordHasher.HashPassword(account, password);
    }
}