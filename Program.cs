using Microsoft.EntityFrameworkCore; // Importing the Entity Framework Core (ORM) for the DbContext, DbSet, Fluent API schema configurations, and async LINQ extension methods.
using MST_4G.Data; // Importing the Data folder where the AppDBContext.cs can be used
using MST_4G.Services; // Importing the Services folder that has the API logic in it

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);// Compatibility configuration switches for Npgsql (PostgreSQL) to accept legacy DateTime handling without strict UTC and infinity conversion errors.
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

var builder = WebApplication.CreateBuilder(args); // This initializes WebApplicationBuilder in ASP.NET Core for setting up the Dependency Injection (Services), configurations (appsettings.json), logging, and web host server of the application.

builder.Services.AddControllers(); // The one that makes the process logic works.
builder.Services.AddOpenApi(); // The one that makes the documentation or blueprint.
builder.Services.AddScoped<IZipService, ZipService>(); // Depdendency Injection container for the service files
builder.Services.AddScoped<ICountyService, CountyService>();

// This registers the AppDbContext to Dependency Injection container using the PostgreSQL provider (Npgsql) and the connection details from the appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
}); // The one that will communicate with the React app

// Building the WebApplication instance using all the services and configurations, for making the HTTP request middleware pipeline ready.
var app = builder.Build();

// Making the OpenAPI endpoint works and Swagger UI for the Development environment only for interactive testing and documentation of the backend APIs.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

// Transferrong "AllowReactApp" CORS policy to HTTP request pipeline for accepting all the requests from the React frontend.
app.UseCors("AllowReactApp");

// It makes the Authorization middleware works for analyzing if there are any permission of requests before accessing all the protected endpoints.
app.UseAuthorization();

// It plots all the HTTP request routes (URL) to go to its Controller action methods.
app.MapControllers();

// This starts the web application and listens to each of the HTTP requests taht enters the system.
app.Run();