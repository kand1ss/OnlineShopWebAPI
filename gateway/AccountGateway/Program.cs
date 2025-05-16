using AccountGateway.Application;
using CatalogManagementGateway.Application.Validators;
using Infrastructure;
using Infrastructure.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InitializeRabbitMQ();
builder.Services.AddSerializers();
builder.Services.AddSingleton<RequestValidator>();
builder.Services.InitializeOpenTelemetry();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, InMemoryCache>();
builder.Services.AddSingleton<RabbitMQPoliciesWrap>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<AccountGateway.AccountGateway>();
app.Run();