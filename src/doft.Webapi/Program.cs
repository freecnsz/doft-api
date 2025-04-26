using doft.Webapi.Extensions;
using doft.Webapi.Middleware;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()  // Minimum logging level
    .WriteTo.Console()     // Log to console
    .WriteTo.File("logs/myapp.log", rollingInterval: RollingInterval.Day)  // Log to a file, rolling daily
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

// Add api layer services
builder.Services.AddControllers();
builder.Services.AddSwaggerConfig();
builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);


// Add identity
builder.Services.ConfigureIdentity();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(
    options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "doft.Webapi v1");
    }
);


app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
