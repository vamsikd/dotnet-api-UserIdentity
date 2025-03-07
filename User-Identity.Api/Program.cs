using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using User_Identity.Api.Data;
using User_Identity.Api.Entities;
using System.Text.Json.Serialization;
using System.Text.Json;
using FluentValidation;
using User_Identity.Api.Filters;
using User_Identity.Api.Middleware;
using Serilog;

// Create builder first
var builder = WebApplication.CreateBuilder(args);

// Now initialize Serilog using builder.Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Retrieve connection string from appSettings (key "IdentiryConnection")
var connectionString = builder.Configuration.GetConnectionString("IdentityConnection");

// Register the DbContext using the connection string
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options
        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .UseSnakeCaseNamingConvention();    
});

// Identity Config
builder.Services.Configure<IdentityOptions>(o => 
{
    // set all the options 
    o.Password.RequireDigit = true;
});
// Identity Services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// register custum filters
builder.Services.AddScoped(typeof(ValidationFilter<>));

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Convert DTO PascalCase to client camelCase
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Register the logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
