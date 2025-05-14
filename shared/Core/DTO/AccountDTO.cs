namespace Core.DTO;

public class AccountDTO
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public DateTime DateOfBirthUtc { get; set; }
}