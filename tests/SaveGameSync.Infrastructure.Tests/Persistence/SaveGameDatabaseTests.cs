using SaveGameSync.Core.Models;
using SaveGameSync.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace SaveGameSync.Infrastructure.Tests.Persistence;

public sealed class SaveGameDatabaseTests
{
    [Fact]
    public async Task ShouldPersistAndLoadTrackedSave()
    {
        var temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            "SaveGameSyncTests",
            Guid.NewGuid().ToString());

        var databasePath = Path.Combine(
            temporaryDirectory,
            "SaveGameSync.db");

        try
        {
            await using (var db = SaveGameDatabaseFactory.Create(databasePath))
            {
                var database = new SaveGameDatabase(db);

                await database.InitializeAsync();

                var trackedSave = new TrackedSave
                {
                    Name = "Dark Souls III",
                    FilePath = @"C:\Test\DS30000.sl2",
                    Exists = true,
                    CurrentHash = "ABC123",
                    LastDetectedAt = DateTimeOffset.UtcNow
                };

                db.TrackedSaves.Add(trackedSave);

                await db.SaveChangesAsync();
            }

            await using (var db = SaveGameDatabaseFactory.Create(databasePath))
            {
                var loadedSave = await db.TrackedSaves
                    .SingleAsync();

                Assert.Equal(
                    "Dark Souls III",
                    loadedSave.Name);

                Assert.Equal(
                    @"C:\Test\DS30000.sl2",
                    loadedSave.FilePath);

                Assert.True(loadedSave.Exists);

                Assert.Equal(
                    "ABC123",
                    loadedSave.CurrentHash);
            }
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            
            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(temporaryDirectory, recursive: true);
            }
        }
    }
}
