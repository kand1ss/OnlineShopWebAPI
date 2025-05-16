using CatalogManagementService.Application;
using CatalogManagementService.Infrastructure;
using Core.Contracts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IRequestDeserializer, RequestDeserializer>();
builder.Services.InitializeRequestProcessors();
builder.Services.InitializeRequestConsumers();
builder.Services.InitializeRabbitMQ();
builder.Services.InitializeOpenTelemetry();
builder.Services.InitializeHealthChecks(builder.Configuration);
builder.Services.AddHostedService<CatalogManagementService.Application.CatalogManagementService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapHealthChecks("/health");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();