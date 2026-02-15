using AgroSolutions.Alerts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AgroSolutions.Alerts.Infrastructure.Data;

public class AlertsDbContext : DbContext
{
    public AlertsDbContext(DbContextOptions<AlertsDbContext> options) : base(options)
    {
    }

    public DbSet<Alert> Alerts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5435;Database=alerts_db;Username=alerts_user;Password=alerts_pass");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FieldId).IsRequired();
            entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Severity).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsResolved).IsRequired();

            entity.HasIndex(e => e.FieldId);
            entity.HasIndex(e => e.IsResolved);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}