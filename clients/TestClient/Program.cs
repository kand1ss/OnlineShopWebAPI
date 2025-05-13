using CatalogManagementGateway;
using Grpc.Net.Client;

class Program
{
    static async Task Main()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:443");
        var client = new CatalogManagementGateway.CatalogManagementGateway.CatalogManagementGatewayClient(channel);

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

    }
}