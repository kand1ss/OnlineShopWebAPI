using AccountManagementService.Domain;

namespace AccountManagementService.Application.Interfaces;

public interface IPasswordService
{
    string HashPassword(Account account, string password);
}