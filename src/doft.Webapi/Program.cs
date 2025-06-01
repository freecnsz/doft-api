using doft.Webapi.Extensions;
using doft.Webapi.Middleware;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// ---------- Configuration ----------
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ---------- Logger Setup ----------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(
        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext:l} | {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/myapp.log", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext:l} | {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

// ---------- Services ----------
builder.Services.AddControllers();
builder.Services.AddSwaggerConfig();
builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddServices(builder.Configuration);

// Authentication and Identity
builder.Services.ConfigureIdentity();
builder.Services.AddJwtAuthentication(builder.Configuration);


var app = builder.Build();

// ---------- Middleware Pipeline ----------

// Swagger for API docs
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "doft.Webapi v1");
});

// Global error handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// HTTPS redirection
app.UseHttpsRedirection();

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map API controllers
app.MapControllers();

app.Run();
