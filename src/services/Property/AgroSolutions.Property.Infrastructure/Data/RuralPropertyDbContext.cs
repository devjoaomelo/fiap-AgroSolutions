using AgroSolutions.Property.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Property.Infrastructure.Data;

public class RuralPropertyDbContext : DbContext
{
    public RuralPropertyDbContext(DbContextOptions<RuralPropertyDbContext> options) : base(options)
    {
    }

    public DbSet<RuralProperty> RuralProperties { get; set; }
    public DbSet<Field> Fields { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=property_db;Username=property_user;Password=property_pass");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RuralProperty>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(300);
            entity.Property(e => e.TotalArea).IsRequired();
            entity.Property(e => e.UserId).IsRequired();

            entity.HasMany(e => e.Fields)
                  .WithOne(f => f.RuralProperty)
                  .HasForeignKey(f => f.RuralPropertyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Culture).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Area).IsRequired();
            entity.Property(e => e.RuralPropertyId).IsRequired();
        });
    }
}