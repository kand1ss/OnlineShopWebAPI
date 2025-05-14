using System.ComponentModel.DataAnnotations;

namespace Core.Requests.Accounts;

public class UpdateAccountRequest(
    Guid id,
    string? firstName, 
    string? lastName, 
    string? surname, 
    string? email, 
    string? password, 
    string? phoneNumber, 
    DateTime? dateOfBirth)
{
    [Required]
    public Guid Id { get; init; } = id;
    
    [MaxLength(50)]
    public string? FirstName { get; } = firstName;
    
    [MaxLength(50)]
    public string? LastName { get; } = lastName;
    
    [MaxLength(50)]
    public string? Surname { get; } = surname;
    
    [MaxLength(50)]
    public string? Email { get; } = email;
    
    [MaxLength(100)]
    public string? Password { get; } = password;
    
    [MaxLength(25)]
    public string? PhoneNumber { get; } = phoneNumber;

    public DateTime? DateOfBirth { get; } = dateOfBirth;
}