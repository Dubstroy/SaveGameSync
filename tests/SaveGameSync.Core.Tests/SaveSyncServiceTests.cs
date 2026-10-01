using SaveGameSync.Core.Models;
using SaveGameSync.Core.Services;

namespace SaveGameSync.Core.Tests;

public class SaveSyncServiceTests
{
    [Fact]
    public async Task SynchronizeAsync_ReturnsFileMissing_WhenSaveDoesNotExist()
    {
        var service = CreateService();

        var trackedSave = new TrackedSave
        {
            Name = "Dark Souls III",
            FilePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString(),
                "DS30000.sl2")
        };

        var result = await service.SynchronizeAsync(
            trackedSave,
            [],
            Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString(),
                "v1.sl2"));

        Assert.Equal(
            SaveSyncStatus.FileMissing,
            result.Status);

        Assert.Null(result.Version);
        Assert.Null(result.CurrentHash);
    }

    [Fact]
    public async Task SynchronizeAsync_ReturnsNoChanges_WhenHashHasNotChanged()
    {
        var service = CreateService();

        var directory = CreateTemporaryDirectory();

        try
        {
            var savePath = Path.Combine(
                directory,
                "DS30000.sl2");

            await File.WriteAllTextAsync(
                savePath,
                "Dark Souls III save data");

            var detector = new SaveFileDetector();

            var currentHash = await detector.CalculateHashAsync(
                savePath);

            var trackedSave = new TrackedSave
            {
                Name = "Dark Souls III",
                FilePath = savePath,
                CurrentHash = currentHash
            };

            var backupPath = Path.Combine(
                directory,
                "Backups",
                "v1.sl2");

            var result = await service.SynchronizeAsync(
                trackedSave,
                [],
                backupPath);

            Assert.Equal(
                SaveSyncStatus.NoChanges,
                result.Status);

            Assert.Equal(
                currentHash,
                result.CurrentHash);

            Assert.Null(result.Version);
            Assert.False(File.Exists(backupPath));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task SynchronizeAsync_CreatesVersionAndBackup_WhenSaveChanged()
    {
        var service = CreateService();

        var directory = CreateTemporaryDirectory();

        try
        {
            var savePath = Path.Combine(
                directory,
                "DS30000.sl2");

            await File.WriteAllTextAsync(
                savePath,
                "New Dark Souls III save data");

            var trackedSave = new TrackedSave
            {
                Name = "Dark Souls III",
                FilePath = savePath,
                CurrentHash = "OLD_HASH"
            };

            var backupPath = Path.Combine(
                directory,
                "Backups",
                "v1.sl2");

            var result = await service.SynchronizeAsync(
                trackedSave,
                [],
                backupPath);

            Assert.Equal(
                SaveSyncStatus.VersionCreated,
                result.Status);

            Assert.NotNull(result.Version);
            Assert.Equal(
                1,
                result.Version.VersionNumber);

            Assert.NotNull(result.CurrentHash);
            Assert.Equal(
                result.CurrentHash,
                result.Version.FileHash);

            Assert.True(File.Exists(backupPath));

            var backupContent =
                await File.ReadAllTextAsync(backupPath);

            Assert.Equal(
                "New Dark Souls III save data",
                backupContent);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task SynchronizeAsync_CreatesNextVersion_WhenPreviousVersionsExist()
    {
        var service = CreateService();

        var directory = CreateTemporaryDirectory();

        try
        {
            var savePath = Path.Combine(
                directory,
                "DS30000.sl2");

            await File.WriteAllTextAsync(
                savePath,
                "Version 3 save");

            var trackedSave = new TrackedSave
            {
                Name = "Dark Souls III",
                FilePath = savePath,
                CurrentHash = "OLD_HASH"
            };

            var existingVersions = new[]
            {
                new SaveVersion
                {
                    TrackedSaveId = trackedSave.Id,
                    VersionNumber = 1,
                    FileHash = "HASH1",
                    BackupPath = Path.Combine(
                        directory,
                        "Backups",
                        "v1.sl2")
                },
                new SaveVersion
                {
                    TrackedSaveId = trackedSave.Id,
                    VersionNumber = 2,
                    FileHash = "HASH2",
                    BackupPath = Path.Combine(
                        directory,
                        "Backups",
                        "v2.sl2")
                }
            };

            var backupPath = Path.Combine(
                directory,
                "Backups",
                "v3.sl2");

            var result = await service.SynchronizeAsync(
                trackedSave,
                existingVersions,
                backupPath);

            Assert.Equal(
                SaveSyncStatus.VersionCreated,
                result.Status);

            Assert.NotNull(result.Version);
            Assert.Equal(
                3,
                result.Version.VersionNumber);

            Assert.True(File.Exists(backupPath));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    private static SaveSyncService CreateService()
    {
        return new SaveSyncService(
            new SaveFileDetector(),
            new SaveChangeDetector(),
            new SaveVersionManager(),
            new SaveBackupService());
    }

    private static string CreateTemporaryDirectory()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(directory);

        return directory;
    }
}
