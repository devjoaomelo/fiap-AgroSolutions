using AgroSolutions.Ingestion.Application.UseCases.GetSensorData;
using AgroSolutions.Ingestion.Application.UseCases.ReceiveSensorData;
using AgroSolutions.Ingestion.Domain.Interfaces;
using AgroSolutions.Ingestion.Infrastructure.Data;
using AgroSolutions.Ingestion.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<IngestionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<ISensorDataRepository, SensorDataRepository>();

// Handlers
builder.Services.AddScoped<ReceiveSensorDataHandler>();
builder.Services.AddScoped<GetSensorDataHandler>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();