using CatalogManagementService.Application;
using CatalogManagementService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.InitializeRequestDeserializers();
builder.Services.InitializeRequestProcessors();
builder.Services.InitializeRabbitMQ();
builder.Services.AddHostedService<CatalogManagementService.Application.CatalogManagementService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();