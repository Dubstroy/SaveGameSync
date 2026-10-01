namespace SaveGameSync.Core.Services;

public sealed class SaveBackupService
{
    public async Task CopyAsync(
        string sourceFilePath,
        string backupFilePath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException(
                "The save file was not found.",
                sourceFilePath);
        }

        var backupDirectory = Path.GetDirectoryName(backupFilePath);

        if (!string.IsNullOrWhiteSpace(backupDirectory))
        {
            Directory.CreateDirectory(backupDirectory);
        }

        await using var sourceStream = new FileStream(
            sourceFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite);

        await using var destinationStream = new FileStream(
            backupFilePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);

        await sourceStream.CopyToAsync(
            destinationStream,
            cancellationToken);
    }
}
