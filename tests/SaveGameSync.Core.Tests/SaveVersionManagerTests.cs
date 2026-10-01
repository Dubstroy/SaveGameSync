using SaveGameSync.Core.Models;
using SaveGameSync.Core.Services;

namespace SaveGameSync.Core.Tests;

public class SaveVersionManagerTests
{
    [Fact]
    public void CreateVersion_StartsAtVersionOne_WhenThereAreNoExistingVersions()
    {
        var manager = new SaveVersionManager();

        var trackedSave = new TrackedSave
        {
            Name = "Dark Souls III",
            FilePath = @"C:\Saves\DS30000.sl2"
        };

        var version = manager.CreateVersion(
            trackedSave,
            "ABC123",
            @"C:\Backups\DS30000.sl2",
            []);

        Assert.Equal(1, version.VersionNumber);
    }

    [Fact]
    public void CreateVersion_IncrementsVersionNumber()
    {
        var manager = new SaveVersionManager();

        var trackedSave = new TrackedSave
        {
            Name = "Dark Souls III",
            FilePath = @"C:\Saves\DS30000.sl2"
        };

        var existingVersions = new[]
        {
            new SaveVersion
            {
                TrackedSaveId = trackedSave.Id,
                VersionNumber = 1,
                FileHash = "HASH1",
                BackupPath = @"C:\Backups\1.sl2"
            },
            new SaveVersion
            {
                TrackedSaveId = trackedSave.Id,
                VersionNumber = 2,
                FileHash = "HASH2",
                BackupPath = @"C:\Backups\2.sl2"
            }
        };

        var version = manager.CreateVersion(
            trackedSave,
            "HASH3",
            @"C:\Backups\3.sl2",
            existingVersions);

        Assert.Equal(3, version.VersionNumber);
    }

    [Fact]
    public void CreateVersion_UsesHighestExistingVersion()
    {
        var manager = new SaveVersionManager();

        var trackedSave = new TrackedSave
        {
            Name = "Dark Souls III",
            FilePath = @"C:\Saves\DS30000.sl2"
        };

        var existingVersions = new[]
        {
            new SaveVersion
            {
                TrackedSaveId = trackedSave.Id,
                VersionNumber = 1,
                FileHash = "HASH1",
                BackupPath = @"C:\Backups\1.sl2"
            },
            new SaveVersion
            {
                TrackedSaveId = trackedSave.Id,
                VersionNumber = 5,
                FileHash = "HASH5",
                BackupPath = @"C:\Backups\5.sl2"
            },
            new SaveVersion
            {
                TrackedSaveId = trackedSave.Id,
                VersionNumber = 3,
                FileHash = "HASH3",
                BackupPath = @"C:\Backups\3.sl2"
            }
        };

        var version = manager.CreateVersion(
            trackedSave,
            "HASH6",
            @"C:\Backups\6.sl2",
            existingVersions);

        Assert.Equal(6, version.VersionNumber);
    }

    [Fact]
    public void CreateVersion_IgnoresVersionsFromOtherSaves()
    {
        var manager = new SaveVersionManager();

        var trackedSave = new TrackedSave
        {
            Name = "Dark Souls III",
            FilePath = @"C:\Saves\DS30000.sl2"
        };

        var otherSave = new TrackedSave
        {
            Name = "Cyberpunk 2077",
            FilePath = @"C:\Saves\Cyberpunk.sav"
        };

        var existingVersions = new[]
        {
            new SaveVersion
            {
                TrackedSaveId = otherSave.Id,
                VersionNumber = 10,
                FileHash = "OTHER",
                BackupPath = @"C:\Backups\other.sl2"
            }
        };

        var version = manager.CreateVersion(
            trackedSave,
            "HASH1",
            @"C:\Backups\1.sl2",
            existingVersions);

        Assert.Equal(1, version.VersionNumber);
    }

    [Fact]
    public void CreateVersion_StoresHashAndBackupPath()
    {
        var manager = new SaveVersionManager();

        var trackedSave = new TrackedSave
        {
            Name = "Dark Souls III",
            FilePath = @"C:\Saves\DS30000.sl2"
        };

        var version = manager.CreateVersion(
            trackedSave,
            "ABC123",
            @"C:\Backups\DS30000-v1.sl2",
            []);

        Assert.Equal("ABC123", version.FileHash);
        Assert.Equal(
            @"C:\Backups\DS30000-v1.sl2",
            version.BackupPath);
    }
}
