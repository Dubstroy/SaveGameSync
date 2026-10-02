using Microsoft.EntityFrameworkCore;
using SaveGameSync.Core.Models;

namespace SaveGameSync.Infrastructure.Persistence;

public sealed class SaveGameDbContext : DbContext
{
    public SaveGameDbContext(
        DbContextOptions<SaveGameDbContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games => Set<Game>();

    public DbSet<TrackedSave> TrackedSaves => Set<TrackedSave>();

    public DbSet<SaveVersion> SaveVersions => Set<SaveVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(game => game.Id);

            entity.Property(game => game.Name)
                .IsRequired();

            entity.Property(game => game.Origin)
                .IsRequired();

            entity.Property(game => game.SaveFolderPath).IsRequired();

            entity.HasMany(game => game.TrackedSaves)
                .WithOne(save => save.Game)
                .HasForeignKey(save => save.GameId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TrackedSave>(entity =>
        {
            entity.HasKey(save => save.Id);

            entity.Property(save => save.GameId)
                .IsRequired();

            entity.Property(save => save.Name)
                .IsRequired();

            entity.Property(save => save.FilePath)
                .IsRequired();

            entity.Property(save => save.CurrentHash);

            entity.Property(save => save.Exists)
                .IsRequired();

            entity.Property(save => save.LastDetectedAt);
        });

        modelBuilder.Entity<SaveVersion>(entity =>
        {
            entity.HasKey(version => version.Id);

            entity.Property(version => version.TrackedSaveId)
                .IsRequired();

            entity.Property(version => version.VersionNumber)
                .IsRequired();

            entity.Property(version => version.FileHash)
                .IsRequired();

            entity.Property(version => version.BackupPath)
                .IsRequired();

            entity.Property(version => version.CreatedAt)
                .IsRequired();

            entity.HasIndex(version => new
            {
                version.TrackedSaveId,
                version.VersionNumber
            })
            .IsUnique();
        });
    }
}
