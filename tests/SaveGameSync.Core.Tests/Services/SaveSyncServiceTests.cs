using SaveGameSync.Core.Models;
using SaveGameSync.Core.Persistence;
using SaveGameSync.Core.Services;

namespace SaveGameSync.Core.Tests.Services;

public class SaveSyncServiceTests
{
    [Fact]
    public async Task SynchronizeAsync_ReturnsFileMissing_WhenSaveDoesNotExist()
    {
        var repository = new InMemorySaveRepository();

        var trackedSave = new TrackedSave
        {
            Name = "Dark Souls III",
            FilePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString(),
                "DS30000.sl2")
        };

        await repository.AddTrackedSaveAsync(trackedSave);

        var service = CreateService(repository);

        var result = await service.SynchronizeAsync(
            trackedSave.Id,
            Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString(),
                "v1.sl2"));

        Assert.Equal(
            SaveSyncStatus.FileMissing,
            result.Status);

        Assert.Null(result.Version);
        Assert.Null(result.CurrentHash);

        var savedTrackedSave =
            await repository.GetTrackedSaveAsync(trackedSave.Id);

        Assert.NotNull(savedTrackedSave);
        Assert.False(savedTrackedSave.Exists);
    }

    [Fact]
    public async Task SynchronizeAsync_ReturnsNoChanges_WhenHashHasNotChanged()
    {
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

            var repository = new InMemorySaveRepository();

            await repository.AddTrackedSaveAsync(trackedSave);

            var service = CreateService(repository);

            var backupPath = Path.Combine(
                directory,
                "Backups",
                "v1.sl2");

            var result = await service.SynchronizeAsync(
                trackedSave.Id,
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

            var repository = new InMemorySaveRepository();

            await repository.AddTrackedSaveAsync(trackedSave);

            var service = CreateService(repository);

            var backupPath = Path.Combine(
                directory,
                "Backups",
                "v1.sl2");

            var result = await service.SynchronizeAsync(
                trackedSave.Id,
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

            var savedTrackedSave =
                await repository.GetTrackedSaveAsync(
                    trackedSave.Id);

            Assert.NotNull(savedTrackedSave);

            Assert.Equal(
                result.CurrentHash,
                savedTrackedSave.CurrentHash);

            var savedVersions =
                await repository.GetVersionsAsync(
                    trackedSave.Id);

            Assert.Single(savedVersions);
            Assert.Equal(
                1,
                savedVersions[0].VersionNumber);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task SynchronizeAsync_CreatesNextVersion_WhenPreviousVersionsExist()
    {
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

            var repository = new InMemorySaveRepository();

            await repository.AddTrackedSaveAsync(trackedSave);

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

            foreach (var version in existingVersions)
            {
                await repository.AddVersionAsync(version);
            }

            var service = CreateService(repository);

            var backupPath = Path.Combine(
                directory,
                "Backups",
                "v3.sl2");

            var result = await service.SynchronizeAsync(
                trackedSave.Id,
                backupPath);

            Assert.Equal(
                SaveSyncStatus.VersionCreated,
                result.Status);

            Assert.NotNull(result.Version);

            Assert.Equal(
                3,
                result.Version.VersionNumber);

            Assert.True(File.Exists(backupPath));

            var versions =
                await repository.GetVersionsAsync(
                    trackedSave.Id);

            Assert.Equal(3, versions.Count);
            Assert.Equal(3, versions[2].VersionNumber);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    private static SaveSyncService CreateService(
        ISaveRepository repository)
    {
        return new SaveSyncService(
            new SaveFileDetector(),
            new SaveChangeDetector(),
            new SaveVersionManager(),
            new SaveBackupService(),
            repository);
    }

    private static string CreateTemporaryDirectory()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(directory);

        return directory;
    }

    private sealed class InMemorySaveRepository : ISaveRepository
    {
        private readonly Dictionary<Guid, TrackedSave> _trackedSaves = [];
        private readonly List<SaveVersion> _versions = [];

        public Task<TrackedSave?> GetTrackedSaveAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            _trackedSaves.TryGetValue(id, out var trackedSave);

            return Task.FromResult(trackedSave);
        }

        public Task<IReadOnlyList<SaveVersion>> GetVersionsAsync(
            Guid trackedSaveId,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<SaveVersion> versions = _versions
                .Where(version =>
                    version.TrackedSaveId == trackedSaveId)
                .OrderBy(version => version.VersionNumber)
                .ToList();

            return Task.FromResult(versions);
        }

        public Task AddTrackedSaveAsync(
            TrackedSave trackedSave,
            CancellationToken cancellationToken = default)
        {
            _trackedSaves[trackedSave.Id] = trackedSave;

            return Task.CompletedTask;
        }

        public Task UpdateTrackedSaveAsync(
            TrackedSave trackedSave,
            CancellationToken cancellationToken = default)
        {
            _trackedSaves[trackedSave.Id] = trackedSave;

            return Task.CompletedTask;
        }

        public Task AddVersionAsync(
            SaveVersion version,
            CancellationToken cancellationToken = default)
        {
            _versions.Add(version);

            return Task.CompletedTask;
        }
    }
}
