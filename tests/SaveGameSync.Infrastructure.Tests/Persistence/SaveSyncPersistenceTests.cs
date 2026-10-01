using Microsoft.Data.Sqlite;
using SaveGameSync.Core.Models;
using SaveGameSync.Core.Services;
using SaveGameSync.Infrastructure.Persistence;

namespace SaveGameSync.Infrastructure.Tests.Persistence;

public sealed class SaveSyncPersistenceTests
{
    [Fact]
    public async Task ShouldMarkSaveAsMissingWithoutDeletingIt()
    {
        var temporaryDirectory = CreateTemporaryDirectory();

        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        var savePath = Path.Combine(
            temporaryDirectory,
            "DS30000.sl2");

        try
        {
            await File.WriteAllTextAsync(
                savePath,
                "Dark Souls III save data");

            await using var db =
                SaveGameDatabaseFactory.Create(databasePath);

            var database = new SaveGameDatabase(db);

            await database.InitializeAsync();

            var repository =
                new SqliteSaveRepository(db);

            var trackedSave = new TrackedSave
            {
                Name = "Dark Souls III",
                FilePath = savePath,
                Exists = true,
                CurrentHash = "OLD_HASH"
            };

            await repository.AddTrackedSaveAsync(
                trackedSave);

            File.Delete(savePath);

            var service = new SaveSyncService(
                new SaveFileDetector(),
                new SaveChangeDetector(),
                new SaveVersionManager(),
                new SaveBackupService(),
                repository);

            var result =
                await service.SynchronizeAsync(
                    trackedSave.Id,
                    Path.Combine(
                        temporaryDirectory,
                        "Backups",
                        "missing.sl2"));

            Assert.Equal(
                SaveSyncStatus.FileMissing,
                result.Status);

            var savedTrackedSave =
                await repository.GetTrackedSaveAsync(
                    trackedSave.Id);

            Assert.NotNull(savedTrackedSave);

            Assert.False(
                savedTrackedSave.Exists);

            Assert.Equal(
                "OLD_HASH",
                savedTrackedSave.CurrentHash);

            Assert.NotNull(
                savedTrackedSave.LastDetectedAt);
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(
                    temporaryDirectory,
                    recursive: true);
            }
        }
    }

    [Fact]
    public async Task ShouldKeepExistingVersionsWhenSaveFileDisappears()
    {
        var temporaryDirectory = CreateTemporaryDirectory();

        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        var savePath = Path.Combine(
            temporaryDirectory,
            "DS30000.sl2");

        var backupPath = Path.Combine(
            temporaryDirectory,
            "Backups",
            "DS30000-v1.sl2");

        try
        {
            await File.WriteAllTextAsync(
                savePath,
                "Dark Souls III save data");

            await using var db =
                SaveGameDatabaseFactory.Create(databasePath);

            var database = new SaveGameDatabase(db);

            await database.InitializeAsync();

            var repository =
                new SqliteSaveRepository(db);

            var trackedSave = new TrackedSave
            {
                Name = "Dark Souls III",
                FilePath = savePath,
                Exists = true
            };

            await repository.AddTrackedSaveAsync(
                trackedSave);

            var service = new SaveSyncService(
                new SaveFileDetector(),
                new SaveChangeDetector(),
                new SaveVersionManager(),
                new SaveBackupService(),
                repository);

            var result =
                await service.SynchronizeAsync(
                    trackedSave.Id,
                    backupPath);

            Assert.Equal(
                SaveSyncStatus.VersionCreated,
                result.Status);

            File.Delete(savePath);

            var missingResult =
                await service.SynchronizeAsync(
                    trackedSave.Id,
                    Path.Combine(
                        temporaryDirectory,
                        "Backups",
                        "missing.sl2"));

            Assert.Equal(
                SaveSyncStatus.FileMissing,
                missingResult.Status);

            var savedVersions =
                await repository.GetVersionsAsync(
                    trackedSave.Id);

            Assert.Single(savedVersions);

            Assert.Equal(
                1,
                savedVersions[0].VersionNumber);

            Assert.Equal(
                result.Version!.FileHash,
                savedVersions[0].FileHash);

            var savedTrackedSave =
                await repository.GetTrackedSaveAsync(
                    trackedSave.Id);

            Assert.NotNull(savedTrackedSave);

            Assert.False(
                savedTrackedSave.Exists);
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(
                    temporaryDirectory,
                    recursive: true);
            }
        }
    }

    [Fact]
    public async Task ShouldCreateVersionAndPersistSynchronization()
    {
        var temporaryDirectory = CreateTemporaryDirectory();

        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        var savePath = Path.Combine(
            temporaryDirectory,
            "DS30000.sl2");

        var backupPath = Path.Combine(
            temporaryDirectory,
            "Backups",
            "DS30000-v1.sl2");

        try
        {
            await File.WriteAllTextAsync(
                savePath,
                "Dark Souls III save data");

            await using var db =
                SaveGameDatabaseFactory.Create(databasePath);

            var database = new SaveGameDatabase(db);

            await database.InitializeAsync();

            var repository =
                new SqliteSaveRepository(db);

            var trackedSave = new TrackedSave
            {
                Name = "Dark Souls III",
                FilePath = savePath,
                Exists = true
            };

            await repository.AddTrackedSaveAsync(
                trackedSave);

            var service = new SaveSyncService(
                new SaveFileDetector(),
                new SaveChangeDetector(),
                new SaveVersionManager(),
                new SaveBackupService(),
                repository);

            var result =
                await service.SynchronizeAsync(
                    trackedSave.Id,
                    backupPath);

            Assert.Equal(
                SaveSyncStatus.VersionCreated,
                result.Status);

            Assert.NotNull(result.Version);

            Assert.Equal(
                1,
                result.Version.VersionNumber);

            Assert.True(
                File.Exists(backupPath));

            var savedTrackedSave =
                await repository.GetTrackedSaveAsync(
                    trackedSave.Id);

            Assert.NotNull(savedTrackedSave);

            Assert.True(
                savedTrackedSave.Exists);

            Assert.False(
                string.IsNullOrWhiteSpace(
                    savedTrackedSave.CurrentHash));

            var savedVersions =
                await repository.GetVersionsAsync(
                    trackedSave.Id);

            Assert.Single(savedVersions);

            Assert.Equal(
                result.Version.Id,
                savedVersions[0].Id);

            Assert.Equal(
                result.CurrentHash,
                savedVersions[0].FileHash);
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(
                    temporaryDirectory,
                    recursive: true);
            }
        }
    }

    private static string CreateTemporaryDirectory()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "SaveGameSyncTests",
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(directory);

        return directory;
    }
}