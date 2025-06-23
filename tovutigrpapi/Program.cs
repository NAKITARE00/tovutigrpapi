using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<CarRepository>();
builder.Services.AddSingleton<DataContext> ();
builder.Services.AddScoped<IStations, StationRepository>();
//builder.Services.AddScoped<UserRepository>();
builder.Services.AddSingleton<DataContext>();
builder.Services.AddScoped<IUsers, UserRepository>();
//builder.ServicesAddScoped<SalesRepository>();
builder.Services.AddSingleton<DataContext>();
builder.Services.AddScoped<IGadgets, GadgetRepository>();
//builder.Services.AddScoped<IIssues>();
builder.Services.AddSingleton<DataContext>();
builder.Services.AddScoped<IIssues, IssueCategoryRepository>();
//builder.Services.AddScoped<ISparePart, SparePartRepository>();
builder.Services.AddScoped<ISparePart, SparePartRepository>();



var app = builder.Build();

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

