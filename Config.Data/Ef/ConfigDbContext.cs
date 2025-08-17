using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Config.Data.Ef;

public partial class ConfigDbContext : DbContext
{
    public ConfigDbContext()
    {
    }

    public ConfigDbContext(DbContextOptions<ConfigDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Configuration> Configuration { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ConfigDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Configur__3214EC07D444F7CB");

            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(sysutcdatetime())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
