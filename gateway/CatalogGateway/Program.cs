using CatalogManagementGateway.Application;
using CatalogManagementGateway.Application.Validators;
using Core.DTO;
using RabbitMQClient.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InitializeRabbitMQ();
builder.Services.AddSerializers();
builder.Services.AddSingleton<MessageRequestClient<ProductDTO>>();
builder.Services.AddSingleton<RequestValidator>();
builder.Services.InitializeOpenTelemetry();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<CatalogGateway.Services.CatalogGateway>();
app.Run();