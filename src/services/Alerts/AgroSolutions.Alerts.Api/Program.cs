using AgroSolutions.Alerts.Application.Events;
using AgroSolutions.Alerts.Application.Services;
using AgroSolutions.Alerts.Application.UseCases.CreateAlert;
using AgroSolutions.Alerts.Application.UseCases.GetAlerts;
using AgroSolutions.Alerts.Application.UseCases.ResolveAlert;
using AgroSolutions.Alerts.Domain.Interfaces;
using AgroSolutions.Alerts.Infrastructure.Data;
using AgroSolutions.Alerts.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AlertsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SensorDataReceivedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAlertProcessingService, AlertProcessingService>();
builder.Services.AddScoped<CreateAlertHandler>();
builder.Services.AddScoped<GetAlertsHandler>();
builder.Services.AddScoped<ResolveAlertHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();