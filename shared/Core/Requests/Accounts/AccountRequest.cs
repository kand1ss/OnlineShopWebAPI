using System.ComponentModel.DataAnnotations;

namespace Core.Requests.Accounts;

public class AccountRequest(
    Guid id,
    string firstName, 
    string lastName, 
    string surname, 
    string email, 
    string password, 
    string phoneNumber, 
    DateTime dateOfBirthUtc)
{
    [Required] public Guid Id { get; set; } = id;
    
    [MaxLength(50)]
    [MinLength(1)]
    [Required]
    public string FirstName { get; } = firstName;
    
    [MaxLength(50)]
    [MinLength(2)]
    [Required]
    public string LastName { get; } = lastName;
    
    [MaxLength(50)]
    [MinLength(2)]
    public string Surname { get; } = surname;
    
    [MaxLength(50)]
    [MinLength(5)]
    [Required]
    [EmailAddress]
    public string Email { get; } = email;
    
    [MaxLength(100)]
    [MinLength(5)]
    [Required]
    public string Password { get; } = password;
    
    [MaxLength(25)]
    [MinLength(10)]
    [Required]
    public string PhoneNumber { get; } = phoneNumber;
    
    [Required]
    public DateTime DateOfBirthUtc { get; } = dateOfBirthUtc;
}