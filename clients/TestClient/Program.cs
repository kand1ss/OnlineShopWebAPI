using AccountGatewayGRPC;
using CatalogGatewayGRPC;
using Grpc.Net.Client;

class Program
{
    static async Task Main()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:443");
        var client = new CatalogGatewayGRPC.CatalogGateway.CatalogGatewayClient(channel);

        var createReply = await client.CreateProductAsync(new CreateProductData
        {
            Title = "Sosiska",
            Description = "Some description",
            Price = "300"
        });
        
        Console.WriteLine($"CREATE: {createReply.Result} - {createReply.Error} - {createReply.Success}");
        var guid = Console.ReadLine();
        if (guid is null)
            return;
        
        var updateReply = await client.UpdateProductAsync(new UpdateProductData
        {
            Id = guid,
            Title = "Somik",
            Description = "Some description2",
            Price = "350"
        });
        
        Console.WriteLine($"UPDATE: {updateReply.Result} - {updateReply.Error} - {updateReply.Success}");
        
        Console.ReadLine();
        
        var removeReply = await client.RemoveProductAsync(new RemoveProductData()
            { Id = guid });
        
        Console.WriteLine($"REMOVE: {removeReply.Result} - {removeReply.Error} - {removeReply.Success}");

        var accountChannel = new AccountGatewayGRPC.AccountGateway.AccountGatewayClient(channel);

        var createAccReply = await accountChannel.CreateAccountAsync(new CreateAccountData()
        {
            FirstName = "Yaroslav",
            LastName = "Varichenko",
            Surname = "Ruslanovich",
            Email = "yaroslav.varichenko@gmail.com",
            Password = "123456789",
            PhoneNumber = "0999999999",
            DateOfBirthUtc = "2007-09-01"
        });
        
        Console.WriteLine($"CREATE: {createAccReply.Success} - {createAccReply.Message} - {createAccReply.AccountId}");
        Console.ReadLine();

        var updateAccReply = await accountChannel.UpdateAccountAsync(new UpdateAccountData()
        {
            Id = createAccReply.AccountId,
            FirstName = "Rockstar",
            DateOfBirthUtc = "2009-09-09"
        });
        
        Console.WriteLine($"UPDATE: {updateAccReply.Success} - {updateAccReply.Message}");
    }
}