using SaveGameSync.Core.Models;

namespace SaveGameSync.Core.Tests.Services;

public class SaveVersionTests
{
    [Fact]
    public void SaveVersion_GeneratesIdAutomatically()
    {
        var version = new SaveVersion
        {
            TrackedSaveId = Guid.NewGuid(),
            VersionNumber = 1,
            FileHash = "ABC123",
            BackupPath = @"C:\Backups\save.sl2"
        };

        Assert.NotEqual(Guid.Empty, version.Id);
    }

    [Fact]
    public void SaveVersion_StoresTrackedSaveId()
    {
        var trackedSaveId = Guid.NewGuid();

        var version = new SaveVersion
        {
            TrackedSaveId = trackedSaveId,
            VersionNumber = 1,
            FileHash = "ABC123",
            BackupPath = @"C:\Backups\save.sl2"
        };

        Assert.Equal(trackedSaveId, version.TrackedSaveId);
    }

    [Fact]
    public void SaveVersion_StoresVersionNumber()
    {
        var version = new SaveVersion
        {
            TrackedSaveId = Guid.NewGuid(),
            VersionNumber = 3,
            FileHash = "ABC123",
            BackupPath = @"C:\Backups\save.sl2"
        };

        Assert.Equal(3, version.VersionNumber);
    }

    [Fact]
    public void SaveVersion_StoresHashAndBackupPath()
    {
        var version = new SaveVersion
        {
            TrackedSaveId = Guid.NewGuid(),
            VersionNumber = 2,
            FileHash = "ABC123",
            BackupPath = @"C:\Backups\save.sl2"
        };

        Assert.Equal("ABC123", version.FileHash);
        Assert.Equal(@"C:\Backups\save.sl2", version.BackupPath);
    }

    [Fact]
    public void SaveVersion_SetsCreationDateAutomatically()
    {
        var before = DateTimeOffset.UtcNow;

        var version = new SaveVersion
        {
            TrackedSaveId = Guid.NewGuid(),
            VersionNumber = 1,
            FileHash = "ABC123",
            BackupPath = @"C:\Backups\save.sl2"
        };

        var after = DateTimeOffset.UtcNow;

        Assert.InRange(version.CreatedAt, before, after);
    }
}
