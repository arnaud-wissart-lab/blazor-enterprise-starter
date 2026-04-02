using Microsoft.EntityFrameworkCore;

namespace BlazorEnterpriseStarter.Server.Infrastructure.Persistence;

/// <summary>
/// Porte le contexte EF Core minimal dédié au module backlog.
/// </summary>
public sealed class BacklogDbContext(DbContextOptions<BacklogDbContext> options) : DbContext(options)
{
    public DbSet<BacklogItemEntity> BacklogItems => Set<BacklogItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var backlogItem = modelBuilder.Entity<BacklogItemEntity>();

        backlogItem.ToTable("BacklogItems");
        backlogItem.HasKey(item => item.Id);

        backlogItem.Property(item => item.Titre)
            .IsRequired()
            .HasMaxLength(160);

        backlogItem.Property(item => item.Description)
            .IsRequired()
            .HasMaxLength(2_000);

        backlogItem.Property(item => item.Statut)
            .HasConversion<string>()
            .HasMaxLength(32);

        backlogItem.Property(item => item.Priorite)
            .HasConversion<string>()
            .HasMaxLength(32);

        backlogItem.Property(item => item.DateCreation)
            .IsRequired();

        backlogItem.HasIndex(item => item.DateCreation);
        backlogItem.HasIndex(item => item.Statut);
        backlogItem.HasIndex(item => item.Priorite);
    }
}
