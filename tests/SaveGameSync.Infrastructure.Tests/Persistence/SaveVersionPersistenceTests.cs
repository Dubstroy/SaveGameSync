using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SaveGameSync.Core.Models;
using SaveGameSync.Infrastructure.Persistence;

namespace SaveGameSync.Infrastructure.Tests.Persistence;

public sealed class SaveVersionPersistenceTests
{
    [Fact]
    public async Task ShouldPersistTrackedSaveAndVersions()
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
            Guid trackedSaveId;

            await using (var db = SaveGameDatabaseFactory.Create(databasePath))
            {
                var database = new SaveGameDatabase(db);

                await database.InitializeAsync();

                var trackedSave = new TrackedSave
                {
                    Name = "Dark Souls III",
                    FilePath = @"C:\Test\DS30000.sl2",
                    Exists = true
                };

                db.TrackedSaves.Add(trackedSave);

                await db.SaveChangesAsync();

                trackedSaveId = trackedSave.Id;

                var version1 = new SaveVersion
                {
                    TrackedSaveId = trackedSaveId,
                    VersionNumber = 1,
                    FileHash = "HASH_001",
                    BackupPath = @"C:\Backups\DS30000\v1.sl2"
                };

                var version2 = new SaveVersion
                {
                    TrackedSaveId = trackedSaveId,
                    VersionNumber = 2,
                    FileHash = "HASH_002",
                    BackupPath = @"C:\Backups\DS30000\v2.sl2"
                };

                db.SaveVersions.AddRange(
                    version1,
                    version2);

                await db.SaveChangesAsync();
            }

            await using (var db = SaveGameDatabaseFactory.Create(databasePath))
            {
                var loadedSave = await db.TrackedSaves
                    .SingleAsync(save => save.Id == trackedSaveId);

                var versions = await db.SaveVersions
                    .Where(version =>
                        version.TrackedSaveId == trackedSaveId)
                    .OrderBy(version => version.VersionNumber)
                    .ToListAsync();

                Assert.Equal(
                    "Dark Souls III",
                    loadedSave.Name);

                Assert.Equal(
                    2,
                    versions.Count);

                Assert.Equal(
                    1,
                    versions[0].VersionNumber);

                Assert.Equal(
                    "HASH_001",
                    versions[0].FileHash);

                Assert.Equal(
                    2,
                    versions[1].VersionNumber);

                Assert.Equal(
                    "HASH_002",
                    versions[1].FileHash);
            }
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
}
