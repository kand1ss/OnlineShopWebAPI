using APIGate.Application;
using APIGate.Application.Validators;
using Core.DTO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InitializeRabbitMQ();
builder.Services.AddDeserializers();
builder.Services.AddSingleton<MessageRequestClient<ProductDTO>>();
builder.Services.AddSingleton<RequestValidator>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<APIGate.Services.CatalogManagementService>();
app.Run();