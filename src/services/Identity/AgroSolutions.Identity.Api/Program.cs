using AgroSolutions.Identity.Application.Services;
using AgroSolutions.Identity.Application.UseCases.Login;
using AgroSolutions.Identity.Application.UseCases.Register;
using AgroSolutions.Identity.Domain.Interfaces;
using AgroSolutions.Identity.Infrastructure.Data;
using AgroSolutions.Identity.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IJwtTokenGenerator>(sp =>
{
    var config = builder.Configuration.GetSection("Jwt");
    return new JwtTokenGenerator(
        secretKey: config["SecretKey"]!,
        issuer: config["Issuer"]!,
        audience: config["Audience"]!,
        expirationMinutes: int.Parse(config["ExpirationMinutes"]!)
    );
});

builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<LoginHandler>();

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