using AgroSolutions.Property.Application.UseCases.CreateField;
using AgroSolutions.Property.Application.UseCases.CreateRuralProperty;
using AgroSolutions.Property.Application.UseCases.DeleteField;
using AgroSolutions.Property.Application.UseCases.DeleteRuralProperty;
using AgroSolutions.Property.Application.UseCases.GetFieldsByProperty;
using AgroSolutions.Property.Application.UseCases.GetRuralProperties;
using AgroSolutions.Property.Domain.Interfaces;
using AgroSolutions.Property.Infrastructure.Data;
using AgroSolutions.Property.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<RuralPropertyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IRuralPropertyRepository, RuralPropertyRepository>();
builder.Services.AddScoped<IFieldRepository, FieldRepository>();

// Handlers
builder.Services.AddScoped<CreateRuralPropertyHandler>();
builder.Services.AddScoped<CreateFieldHandler>();
builder.Services.AddScoped<GetRuralPropertiesHandler>();
builder.Services.AddScoped<GetFieldsByPropertyHandler>();
builder.Services.AddScoped<DeleteRuralPropertyHandler>();
builder.Services.AddScoped<DeleteFieldHandler>();

// Controllers
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