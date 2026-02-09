using AgroSolutions.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Identity.Infrastructure.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base()
    {
    }

    public DbSet<User> Users { get; set; }
     
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(e => e.Id);
            e.HasIndex(e => e.Email).IsUnique();
            e.Property(e => e.Email).IsRequired().HasMaxLength(255);
            e.Property(e => e.Email).IsRequired().HasMaxLength(200);
            e.Property(e => e.PasswordHash).IsRequired();
        });
    }
}