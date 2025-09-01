using MySqlConnector;
using System.Data;
using System.Text.RegularExpressions;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Repositories;
using tovutigrpapi.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories and DataContext
builder.Services.AddSingleton<DataContext>();
builder.Services.AddScoped<IStations, StationRepository>();
builder.Services.AddScoped<IUsers, UserRepository>();
builder.Services.AddScoped<IGadgets, GadgetRepository>();
builder.Services.AddScoped<IIssues, IssueCategoryRepository>();
builder.Services.AddScoped<ISparePart, SparePartRepository>();
builder.Services.AddScoped<IClients, ClientRepository>();

builder.Services.AddScoped<IDbConnection>(sp =>
    new MySqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<AuthorizationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://sahara-assets.vercel.app",
                "https://assets.saharafcs.com",
                "https://assetsapi.saharafcs.com",
                "http://127.0.0.1:8000/"
        )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
