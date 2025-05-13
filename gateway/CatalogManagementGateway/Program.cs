using CatalogManagementGateway.Application;
using CatalogManagementGateway.Application.Validators;
using Core.DTO;
using RabbitMQClient.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InitializeRabbitMQ();
builder.Services.AddDeserializers();
builder.Services.AddSingleton<MessageRequestClient<ProductDTO>>();
builder.Services.AddSingleton<RequestValidator>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<CatalogManagementGateway.Services.CatalogManagementGateway>();
app.Run();