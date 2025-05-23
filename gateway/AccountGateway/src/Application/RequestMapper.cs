using AccountGatewayGRPC;
using Core.Requests.Accounts;

namespace AccountGateway.Application;

public static class RequestMapper
{
    private static DateTime ParseDateTime(string date)
        => DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Utc);
    
    public static CreateAccountRequest MapToRequest(this CreateAccountData data)
        => new(Guid.Parse(data.TempUserId), data.FirstName, data.LastName, data.Surname, data.Email, 
            data.Password, data.PhoneNumber, ParseDateTime(data.DateOfBirthUtc));
    
    public static UpdateAccountRequest MapToRequest(this UpdateAccountData data)
        => new(Guid.Parse(data.UserId), data.FirstName, data.LastName, data.Surname, data.Email, 
            data.Password, data.PhoneNumber, ParseDateTime(data.DateOfBirthUtc));
}