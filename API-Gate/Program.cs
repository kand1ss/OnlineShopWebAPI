using APIGate.Application.Deserializers;
using APIGate.Hosted;
using CatalogManagementService.Application.Replies;
using Core.Contracts;
using RabbitMQClient;
using RabbitMQClient.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionService, RabbitMQConnectionService>();
builder.Services.AddSingleton<IRabbitMQClient, RabbitMQClient.RabbitMQClient>();
builder.Services.AddSingleton<IMessageDeserializer<byte[], ProductOperationReply>, ProductReplyDeserializer>();
builder.Services.AddHostedService<RabbitMQInitializer>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<APIGate.Services.CatalogManagementService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();