using AccountGateway.Application;
using CatalogManagementGateway.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InitializeRabbitMQ();
builder.Services.AddSerializers();
builder.Services.AddSingleton<RequestValidator>();
builder.Services.InitializeOpenTelemetry();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<AccountGateway.AccountGateway>();
app.Run();