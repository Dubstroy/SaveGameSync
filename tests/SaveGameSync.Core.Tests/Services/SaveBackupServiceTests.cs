using SaveGameSync.Core.Services;

namespace SaveGameSync.Core.Tests.Services;

public class SaveBackupServiceTests
{
    [Fact]
    public async Task CopyAsync_CopiesFileContents()
    {
        var service = new SaveBackupService();

        var directory = CreateTemporaryDirectory();

        try
        {
            var sourcePath = Path.Combine(directory, "source.sl2");
            var backupPath = Path.Combine(directory, "backup.sl2");

            var originalContent =
                "Dark Souls III test save data";

            await File.WriteAllTextAsync(
                sourcePath,
                originalContent);

            await service.CopyAsync(
                sourcePath,
                backupPath);

            var backupContent =
                await File.ReadAllTextAsync(backupPath);

            Assert.Equal(
                originalContent,
                backupContent);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task CopyAsync_CreatesDestinationDirectory()
    {
        var service = new SaveBackupService();

        var directory = CreateTemporaryDirectory();

        try
        {
            var sourcePath = Path.Combine(
                directory,
                "source.sl2");

            var backupDirectory = Path.Combine(
                directory,
                "Backups",
                "DarkSoulsIII");

            var backupPath = Path.Combine(
                backupDirectory,
                "v1.sl2");

            await File.WriteAllTextAsync(
                sourcePath,
                "test save");

            await service.CopyAsync(
                sourcePath,
                backupPath);

            Assert.True(
                File.Exists(backupPath));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task CopyAsync_ThrowsWhenSourceDoesNotExist()
    {
        var service = new SaveBackupService();

        var directory = CreateTemporaryDirectory();

        try
        {
            var sourcePath = Path.Combine(
                directory,
                "missing.sl2");

            var backupPath = Path.Combine(
                directory,
                "backup.sl2");

            await Assert.ThrowsAsync<FileNotFoundException>(
                () => service.CopyAsync(
                    sourcePath,
                    backupPath));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task CopyAsync_DoesNotModifySourceFile()
    {
        var service = new SaveBackupService();

        var directory = CreateTemporaryDirectory();

        try
        {
            var sourcePath = Path.Combine(
                directory,
                "source.sl2");

            var backupPath = Path.Combine(
                directory,
                "backup.sl2");

            var originalContent =
                "original save content";

            await File.WriteAllTextAsync(
                sourcePath,
                originalContent);

            await service.CopyAsync(
                sourcePath,
                backupPath);

            var sourceContent =
                await File.ReadAllTextAsync(sourcePath);

            Assert.Equal(
                originalContent,
                sourceContent);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
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
