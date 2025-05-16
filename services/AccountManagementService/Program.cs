using AccountManagementService.Application;
using AccountManagementService.Infrastructure;
using AccountManagementService.Infrastructure.Context;
using CatalogManagementService.Application;
using Core.Contracts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.InitializeRabbitMQ();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IRequestDeserializer, RequestDeserializer>();
builder.Services.AddSingleton<RequestValidator>();
builder.Services.AddRequestProcessors();
builder.Services.AddRequestConsumers();
builder.Services.InitializeOpenTelemetry();
builder.Services.InitializeHealthChecks(builder.Configuration);
builder.Services.AddHostedService<AccountManagementService.AccountManagementService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapHealthChecks("/health");
app.UseHttpsRedirection();
app.UseAuthorization();
app.Run();