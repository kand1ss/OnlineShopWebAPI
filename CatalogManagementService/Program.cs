using CatalogManagementService.Application;
using CatalogManagementService.Infrastructure;
using RabbitMQClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSingleton<IRabbitMQClient, RabbitMQClient.RabbitMQClient>();
builder.Services.AddHostedService<CatalogManagementService.Application.CatalogManagementService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();