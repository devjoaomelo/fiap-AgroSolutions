using AgroSolutions.Property.Application.UseCases.CreateField;
using AgroSolutions.Property.Application.UseCases.CreateRuralProperty;
using AgroSolutions.Property.Application.UseCases.GetProperties;
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
builder.Services.AddScoped<GetPropertiesHandler>();

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