using AgroSolutions.Ingestion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AgroSolutions.Ingestion.Infrastructure.Data;

public class IngestionDbContext : DbContext
{
    public IngestionDbContext(DbContextOptions<IngestionDbContext> options) : base(options)
    {
    }

    public DbSet<SensorData> SensorData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5434;Database=ingestion_db;Username=ingestion_user;Password=ingestion_pass");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SensorData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FieldId).IsRequired();
            entity.Property(e => e.SoilMoisture).IsRequired();
            entity.Property(e => e.Temperature).IsRequired();
            entity.Property(e => e.Precipitation).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.FieldId);
            entity.HasIndex(e => e.Timestamp);
        });
    }
}