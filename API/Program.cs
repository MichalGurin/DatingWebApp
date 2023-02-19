using API.Data;
using API.Extensions;
using API.Middleware;
using API.Models;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{

}

app.UseCors(builder => builder
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials()   //needed for SignalR
.WithOrigins("http://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");     //needed for SignalR
app.MapHub<MessageHub>("hubs/message");     //needed for SignalR

app.MapFallbackToController("Index", "Fallback");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
      var context = services.GetRequiredService<DataContext>();
      await context.Database.MigrateAsync();
      await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]"); //Clear all message connection during startup
      await Seed.SeedUsers(
            services.GetRequiredService<UserManager<AppUser>>(),
            services.GetRequiredService<RoleManager<AppRole>>());
}
catch (System.Exception ex)
{
      var logger = services.GetService<ILogger<Program>>();
      logger.LogError(ex, "An error occured during migration");
}

app.Run();
