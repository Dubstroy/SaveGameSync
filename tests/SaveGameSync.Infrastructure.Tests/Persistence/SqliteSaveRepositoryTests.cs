using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SaveGameSync.Core.Models;
using SaveGameSync.Infrastructure.Persistence;

namespace SaveGameSync.Infrastructure.Tests.Persistence;

public sealed class SqliteSaveRepositoryTests
{
    [Fact]
    public async Task ShouldAddAndRetrieveTrackedSave()
    {
        var temporaryDirectory = CreateTemporaryDirectory();
        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        try
        {
            await using var db =
                SaveGameDatabaseFactory.Create(databasePath);

            var database = new SaveGameDatabase(db);
            await database.InitializeAsync();

            var repository = new SqliteSaveRepository(db);

            var game = new Game
            {
                Name = "Dark Souls III"
            };

            db.Games.Add(game);
            await db.SaveChangesAsync();

            var trackedSave = new TrackedSave
            {
                GameId = game.Id,
                Name = "Dark Souls III",
                FilePath = @"C:\Test\DS30000.sl2",
                Exists = true,
                CurrentHash = "HASH_001"
            };

            await repository.AddTrackedSaveAsync(trackedSave);

            var loadedSave =
                await repository.GetTrackedSaveAsync(
                    trackedSave.Id);

            Assert.NotNull(loadedSave);
            Assert.Equal(
                trackedSave.Id,
                loadedSave.Id);

            Assert.Equal(
                "Dark Souls III",
                loadedSave.Name);

            Assert.Equal(
                "HASH_001",
                loadedSave.CurrentHash);
        }
        finally
        {
            CleanupTemporaryDirectory(temporaryDirectory);
        }
    }

    [Fact]
    public async Task ShouldUpdateTrackedSave()
    {
        var temporaryDirectory = CreateTemporaryDirectory();
        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        try
        {
            await using var db =
                SaveGameDatabaseFactory.Create(databasePath);

            var database = new SaveGameDatabase(db);
            await database.InitializeAsync();

            var repository = new SqliteSaveRepository(db);

            var game = new Game
            {
                Name = "Dark Souls III"
            };

            db.Games.Add(game);
            await db.SaveChangesAsync();

            var trackedSave = new TrackedSave

            {
                GameId = game.Id,
                Name = "Dark Souls III",
                FilePath = @"C:\Test\DS30000.sl2",
                Exists = true,
                CurrentHash = "HASH_001"
            };

            await repository.AddTrackedSaveAsync(trackedSave);

            trackedSave.Exists = false;
            trackedSave.CurrentHash = null;
            trackedSave.LastDetectedAt =
                DateTimeOffset.UtcNow;

            await repository.UpdateTrackedSaveAsync(
                trackedSave);

            var loadedSave =
                await repository.GetTrackedSaveAsync(
                    trackedSave.Id);

            Assert.NotNull(loadedSave);
            Assert.False(loadedSave.Exists);
            Assert.Null(loadedSave.CurrentHash);
            Assert.NotNull(loadedSave.LastDetectedAt);
        }
        finally
        {
            CleanupTemporaryDirectory(temporaryDirectory);
        }
    }

    [Fact]
    public async Task ShouldAddAndRetrieveVersionsInOrder()
    {
        var temporaryDirectory = CreateTemporaryDirectory();
        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        try
        {
            await using var db =
                SaveGameDatabaseFactory.Create(databasePath);

            var database = new SaveGameDatabase(db);
            await database.InitializeAsync();

            var repository = new SqliteSaveRepository(db);

            var game = new Game
            {
                Name = "Dark Souls III"
            };

            db.Games.Add(game);
            await db.SaveChangesAsync();

            var trackedSave = new TrackedSave
            {
                GameId = game.Id,
                Name = "Dark Souls III",
                FilePath = @"C:\Test\DS30000.sl2",
                Exists = true
            };

            await repository.AddTrackedSaveAsync(
                trackedSave);

            var version2 = new SaveVersion
            {
                TrackedSaveId = trackedSave.Id,
                VersionNumber = 2,
                FileHash = "HASH_002",
                BackupPath = @"C:\Backups\v2.sl2"
            };

            var version1 = new SaveVersion
            {
                TrackedSaveId = trackedSave.Id,
                VersionNumber = 1,
                FileHash = "HASH_001",
                BackupPath = @"C:\Backups\v1.sl2"
            };

            await repository.AddVersionAsync(version2);
            await repository.AddVersionAsync(version1);

            var versions =
                await repository.GetVersionsAsync(
                    trackedSave.Id);

            Assert.Equal(2, versions.Count);

            Assert.Equal(
                1,
                versions[0].VersionNumber);

            Assert.Equal(
                2,
                versions[1].VersionNumber);

            Assert.Equal(
                "HASH_001",
                versions[0].FileHash);

            Assert.Equal(
                "HASH_002",
                versions[1].FileHash);
        }
        finally
        {
            CleanupTemporaryDirectory(temporaryDirectory);
        }
    }

    [Fact]
    public async Task ShouldKeepTrackedSaveWhenFileIsMissing()
    {
        var temporaryDirectory = CreateTemporaryDirectory();
        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        try
        {
            await using var db =
                SaveGameDatabaseFactory.Create(databasePath);

            var database = new SaveGameDatabase(db);
            await database.InitializeAsync();

            var repository = new SqliteSaveRepository(db);

            var game = new Game
            {
                Name = "Dark Souls III"
            };

            db.Games.Add(game);
            await db.SaveChangesAsync();

            var trackedSave = new TrackedSave
            {
                GameId = game.Id,
                Name = "Dark Souls III",
                FilePath = @"C:\Test\DS30000.sl2",
                Exists = true,
                CurrentHash = "HASH_001"
            };

            await repository.AddTrackedSaveAsync(
                trackedSave);

            trackedSave.Exists = false;
            trackedSave.LastDetectedAt =
                DateTimeOffset.UtcNow;

            await repository.UpdateTrackedSaveAsync(
                trackedSave);

            var loadedSave =
                await repository.GetTrackedSaveAsync(
                    trackedSave.Id);

            Assert.NotNull(loadedSave);

            Assert.Equal(
                trackedSave.Id,
                loadedSave.Id);

            Assert.False(loadedSave.Exists);

            Assert.Equal(
                "HASH_001",
                loadedSave.CurrentHash);
        }
        finally
        {
            CleanupTemporaryDirectory(temporaryDirectory);
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

    private static void CleanupTemporaryDirectory(
        string directory)
    {
        SqliteConnection.ClearAllPools();

        if (Directory.Exists(directory))
        {
            Directory.Delete(
                directory,
                recursive: true);
        }
    }
}
