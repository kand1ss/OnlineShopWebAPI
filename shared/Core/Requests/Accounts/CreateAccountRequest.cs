namespace Core.Requests.Accounts;

public class CreateAccountRequest(
    Guid id,
    string firstName, 
    string lastName, 
    string surname, 
    string email, 
    string password, 
    string phoneNumber, 
    DateTime dateOfBirthUtc) 
    : AccountRequest(id, firstName, lastName, surname, email, password, phoneNumber, dateOfBirthUtc);