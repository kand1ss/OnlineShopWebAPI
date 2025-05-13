using CatalogManagementService.Application;
using CatalogManagementService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.InitializeRequestDeserializers();
builder.Services.InitializeRequestProcessors();
builder.Services.InitializeRequestConsumers();
builder.Services.InitializeRabbitMQ();
builder.Services.AddHostedService<CatalogManagementService.Application.CatalogManagementService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();