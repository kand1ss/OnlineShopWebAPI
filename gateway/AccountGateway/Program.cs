using CatalogManagementGateway.Application.Deserializers;
using CatalogManagementGateway.Hosted;
using CatalogManagementService.Application;
using Core.Contracts;
using OpenTelemetry.Trace;
using RabbitMQClient;
using RabbitMQClient.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRabbitMQClient, RabbitMQClient.RabbitMQClient>();
builder.Services.AddSingleton<IConnectionService, RabbitMQConnectionService>();
builder.Services.AddHostedService<RabbitMQInitializer>();
builder.Services.AddSingleton<IRequestDeserializer, RequestDeserializer>();
builder.Services.AddScoped<IRequestSerializer<byte[]>, RequestSerializer>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddGrpcClientInstrumentation()
            .AddConsoleExporter();
    });

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<AccountGateway.AccountGateway>();
app.Run();