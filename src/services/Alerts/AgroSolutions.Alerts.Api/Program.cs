using AgroSolutions.Alerts.Application.Services;
using AgroSolutions.Alerts.Application.UseCases.CreateAlert;
using AgroSolutions.Alerts.Application.UseCases.GetAlerts;
using AgroSolutions.Alerts.Application.UseCases.ResolveAlert;
using AgroSolutions.Alerts.Domain.Interfaces;
using AgroSolutions.Alerts.Infrastructure.Data;
using AgroSolutions.Alerts.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AlertsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IAlertRepository, AlertRepository>();

// Services
builder.Services.AddScoped<IAlertProcessingService, AlertProcessingService>();

// Handlers
builder.Services.AddScoped<CreateAlertHandler>();
builder.Services.AddScoped<GetAlertsHandler>();
builder.Services.AddScoped<ResolveAlertHandler>();

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